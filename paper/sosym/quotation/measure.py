"""Time the quotation scenarios of the SoSyM paper against a local IPFees REST service.

Usage: python measure.py [api_base] [out_dir]
Defaults to http://localhost:18090/api/v1 and the folder this script is in.

The filing is the paper's running example (large entity, 20 claims over 35 pages, one priority,
5,000 words to translate), with every other input set to the value the schedules declare as their
default, so the API answers what the web form would. Three scenarios are timed:
  quote20  -- one quotation over twenty offices chosen across the main markets and regions;
  quoteAll -- one quotation over every office that serves a schedule;
  explain20 -- the explanation of the 20-office quotation with what-if alternatives.
Each is run WARMUP times untimed and RUNS times timed, one request at a time.
"""
import json
import statistics
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

API = sys.argv[1] if len(sys.argv) > 1 else 'http://localhost:18090/api/v1'
OUT = Path(sys.argv[2]) if len(sys.argv) > 2 else Path(__file__).resolve().parent
WARMUP, RUNS = 3, 30

# The author's choice of twenty offices across the main markets and the regions the corpus covers.
OFFICES_20 = ['US', 'EP', 'JP', 'KR', 'CA', 'AU', 'IN', 'BR', 'MX', 'IL',
              'SG', 'ZA', 'NZ', 'EA', 'CO', 'ID', 'SA', 'AE', 'TH', 'CN']

INPUTS = [
    ('String', 'EntitySize', 'Entity_Large'),
    ('Date', 'ApplicationDate', '2026-10-16'),
    ('Number', 'ClaimCount', '20'),
    ('Number', 'SheetCount', '35'),
    ('Number', 'PriorityCount', '1'),
    ('Number', 'IndependentClaimCount', '3'),
    ('Number', 'MultipleDependentClaimCount', '0'),
    ('Number', 'DependentClaimCount', '17'),
    ('Number', 'CountryCount', '1'),
    ('String', 'ISA', 'ISA_EPO'),
    ('String', 'IPRP', 'IPRP_NONE'),
    ('Boolean', 'ISR_AVAILABLE', 'true'),
    ('Boolean', 'RO_NP', 'false'),
    ('Boolean', 'OfficeElected', 'false'),
    ('Boolean', 'Extension', 'false'),
    ('Number', 'NumberOfWords', '5000'),
    ('MultipleStrings', 'VAL_CTY', 'None'),
]


def call(method, path, body=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(API + path, data=data, method=method,
                                 headers={'Content-Type': 'application/json'})
    started = time.perf_counter()
    try:
        with urllib.request.urlopen(req) as resp:
            payload = json.load(resp)
            status = resp.status
    except urllib.error.HTTPError as e:
        payload, status = e.read().decode(errors='replace'), e.code
    return status, payload, (time.perf_counter() - started) * 1000


def request(offices, currency='EUR'):
    return {'Jurisdictions': ','.join(offices), 'TargetCurrency': currency,
            'Parameters': [{'Type': t, 'Name': n, 'Value': v} for t, n, v in INPUTS]}


def timed(method, path, body):
    for _ in range(WARMUP):
        status, payload, _ = call(method, path, body)
        if status != 200:
            raise SystemExit(f'{path} answered {status}: {str(payload)[:300]}')
    times = [call(method, path, body)[2] for _ in range(RUNS)]
    times.sort()
    return payload, {'runs': RUNS, 'median_ms': round(statistics.median(times), 1),
                     'p95_ms': round(times[int(0.95 * (RUNS - 1))], 1), 'max_ms': round(times[-1], 1)}


def main():
    _, jurisdictions, _ = call('GET', '/Jurisdiction')
    codes = [j['jurisdiction'] for j in jurisdictions]
    served = [c for c in codes if call('POST', '/Fee/Calculate', request([c]))[0] == 200]
    _, params, _ = call('GET', '/Fee/Parameters/' + ','.join(served))

    quote20, t20 = timed('POST', '/Fee/Calculate', request(OFFICES_20))
    quote_all, t_all = timed('POST', '/Fee/Calculate', request(served))
    explain20, t_explain = timed('POST', '/Fee/Explain?alternatives=true', request(OFFICES_20))

    # Quote time against the number of offices: the twenty first, then the remaining served offices
    # in code order.
    ordered = OFFICES_20 + sorted(c for c in served if c not in OFFICES_20)
    scaling = []
    for n in (1, 5, 10, 20, 40, 80, len(served)):
        _, t = timed('POST', '/Fee/Calculate', request(ordered[:n]))
        _, e = timed('POST', '/Fee/Explain', request(ordered[:n]))
        scaling.append({'offices': n, 'quote': t, 'explain': e})

    alternatives = sum(len(e.get('alternatives') or e.get('Alternatives') or []) for e in explain20)
    results = {
        'api': API,
        'inputs': {n: v for _, n, v in INPUTS},
        'offices_registered': len(codes),
        'offices_served': len(served),
        'not_served': sorted(set(codes) - set(served)),
        'questionnaire_inputs_all_served': len(params['inputs']),
        'quote20': {'offices': OFFICES_20, 'timing': t20, 'response': quote20},
        'quoteAll': {'offices': len(served), 'timing': t_all,
                     'grand_total': quote_all.get('grandTotalFee') or quote_all.get('GrandTotalFee')},
        'explain20': {'timing': t_explain, 'explanations': len(explain20), 'alternatives': alternatives},
        'scaling': scaling,
    }
    rows = ['| Office | Official, mandatory | Optional (translation, agent, elective) | Service charge | Office total, mandatory |',
            '|---|---:|---:|---:|---:|']
    for j in quote20['jurisdictionFees']:
        rows.append(f"| {j['jurisdiction']} | {j['fees']['mandatoryAmount']:,.2f} | {j['fees']['optionalAmount']:,.2f}"
                    f" | {j['serviceFee']['mandatoryAmount']:,.2f} | {j['totalFee']['mandatoryAmount']:,.2f} |")
    f, s, g = quote20['totalFees'], quote20['totalServiceFee'], quote20['grandTotalFee']
    rows.append(f"| **Total (EUR)** | **{f['mandatoryAmount']:,.2f}** | **{f['optionalAmount']:,.2f}**"
                f" | **{s['mandatoryAmount']:,.2f}** | **{g['mandatoryAmount']:,.2f}** |")
    (OUT / 'quote20.md').write_text('\n'.join(rows) + '\n', encoding='utf-8')
    srows = ['| Offices | Quote, median (ms) | Quote, 95th pct (ms) | Explanation, median (ms) | Explanation, 95th pct (ms) |',
             '|---:|---:|---:|---:|---:|']
    srows += [f"| {s['offices']} | {s['quote']['median_ms']} | {s['quote']['p95_ms']} | {s['explain']['median_ms']} | {s['explain']['p95_ms']} |"
              for s in scaling]
    (OUT / 'scaling.md').write_text('\n'.join(srows) + '\n', encoding='utf-8')

    (OUT / 'results.json').write_text(json.dumps(results, indent=1, ensure_ascii=False), encoding='utf-8')
    (OUT / 'explain20.json').write_text(json.dumps(explain20, indent=1, ensure_ascii=False), encoding='utf-8')
    print(json.dumps({k: v for k, v in results.items() if k != 'quote20'}, indent=1)[:3000])
    print('quote20 timing', t20)


if __name__ == '__main__':
    main()
