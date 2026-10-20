"""Split the optional amounts of the twenty-office quotation (results.json) into elective official
fees and the corpus's uniform placeholders for translation and the agent's fee (explain20.json),
and write quote20_split.md, the source of Table 2.

The placeholders are priced in EUR in every schedule, so their EUR amounts come straight from the
explanation; the elective official fees are what remains of each office's optional total after
conversion.
"""
import json
from pathlib import Path

HERE = Path(__file__).resolve().parent
quote = json.loads((HERE / 'results.json').read_text(encoding='utf-8'))['quote20']['response']
explain = {e['feeName'].removeprefix('PCT-'): e for e in json.loads((HERE / 'explain20.json').read_text(encoding='utf-8'))}
PLACEHOLDERS = ('TranslationFee', 'AgentServiceFee')

rows = ['| Office | Official, mandatory | Official, elective | Translation and agent (placeholders) | Service charge | Office total, mandatory |',
        '|---|---:|---:|---:|---:|---:|']
totals = [0, 0, 0, 0, 0]
for j in quote['jurisdictionFees']:
    code = j['jurisdiction']
    fees = explain[code]['fees']
    assert all(f['currency'] == 'EUR' for f in fees if f['name'] in PLACEHOLDERS), code
    placeholder = sum(f['amount'] for f in fees if f['name'] in PLACEHOLDERS)
    elective = j['fees']['optionalAmount'] - placeholder
    values = [j['fees']['mandatoryAmount'], elective, placeholder, j['serviceFee']['mandatoryAmount'], j['totalFee']['mandatoryAmount']]
    totals = [a + b for a, b in zip(totals, values)]
    rows.append(f'| {code} | ' + ' | '.join(f'{v:,.0f}' for v in values) + ' |')
rows.append('| Total | ' + ' | '.join(f'{v:,.0f}' for v in totals) + ' |')
(HERE / 'quote20_split.md').write_text('\n'.join(rows) + '\n', encoding='utf-8')
print('\n'.join(rows))
