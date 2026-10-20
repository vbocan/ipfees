"""Tally the batch records.

Usage: python tally.py   (prints per-office rows and the totals used in Section 6.2)
"""
import collections
import glob
import re


rows = []
for path in sorted(glob.glob("batch-*.md")):
    office = None
    for line in open(path, encoding="utf-8"):
        m = re.match(r"^## ([A-Z]{2}):", line)
        if m:
            office = m.group(1)
            continue
        cells = [c.strip() for c in line.strip().strip("|").split("|")]
        if cells and cells[0] == "Office":
            office = None  # the batch summary table
        if office and line.startswith("|") and len(cells) >= 7 and not set(cells[0]) <= set("-: "):
            if cells[0] in ("Fee", "Office") or cells[5] == "Verdict":
                continue
            fee, cond, verdict, effective = cells[0], cells[1], cells[5], cells[4]
            rows.append((office, fee, cond, verdict, effective))

kind = lambda v: re.match(r"[A-Z]+", v.lstrip("*")).group(0)
by_office = collections.defaultdict(collections.Counter)
for o, f, c, v, e in rows:
    by_office[o][kind(v)] += 1
tot = collections.Counter()
for o in sorted(by_office):
    tot.update(by_office[o])
print("offices", len(by_office))
print("totals", dict(tot))
checked = sum(tot[k] for k in ("MATCH", "UPDATED", "REMOVED", "UNVERIFIED"))
print("fee lines checked", checked)
upd = [r for r in rows if kind(r[3]) == "UPDATED"]
sub = collections.Counter()
for r in upd:
    for s in re.findall(r"amount|threshold|condition|basis|currency", r[3]):
        sub[s] += 1
print("updated by kind (a row may carry several)", dict(sub))
nonamount = [r for r in upd if re.search(r"threshold|condition|basis", r[3])]
print("updated rows changing more than an amount:", len(nonamount))
for r in nonamount:
    print("   ", r[0], r[1], "|", r[3])
before = [r for r in upd if re.search(r"19\d\d|20(0\d|1\d|2[0-3])", r[4]) and not re.search(r"202[4-6]", r[4])]
print("updated rows whose in-force amount predates 2024:", len(before))
changed = sorted({r[0] for r in rows if kind(r[3]) in ("UPDATED", "REMOVED")})
print("schedules with changes", len(changed))
unver = sorted({r[0] for r in rows if kind(r[3]) == "UNVERIFIED"})
print("schedules with unverified rows", len(unver), unver)

# By region (Table 3 of the paper).
REGIONS = {
    "Regional offices": "AP EA EP OA",
    "Europe": "AL AT BA BG BY CH CZ DE DK EE ES FI GB HR HU IS IT LU MD MK NO PL PT RO RS RU SE SK TR UA",
    "Asia and the Pacific": "AM AU AZ BN CN GE ID IN JP KG KH KP KR KZ LA LK MN NZ PG PH SG TH TJ TM UZ VN WS",
    "Middle East and North Africa": "AE BH DJ DZ EG IL IQ IR JO KW MA OM QA SA SD SY TN",
    "Sub-Saharan Africa": "AO BW GH GM KE LR LS MG MW MZ NA NG SC TZ UG ZA ZM ZW",
    "Americas": "AG BB BR BZ CA CO CR CU DO EC GT HN JM KN MX NI PA PE SV TT US VC",
}
region_of = {c: r for r, cs in REGIONS.items() for c in cs.split()}
assert set(region_of) == set(by_office), set(by_office) ^ set(region_of)
print("\n| Region | Schedules | Fee lines | Match | Updated | Removed | Unverified | Not modelled | Schedules changed |")
print("|---|---:|---:|---:|---:|---:|---:|---:|---:|")
grand = collections.Counter()
for region, codes in REGIONS.items():
    c = collections.Counter()
    for code in codes.split():
        c.update(by_office[code])
    n_changed = sum(1 for code in codes.split() if code in changed)
    lines_ = sum(c[k] for k in ("MATCH", "UPDATED", "REMOVED", "UNVERIFIED"))
    print(f"| {region} | {len(codes.split())} | {lines_} | {c['MATCH']} | {c['UPDATED']} | {c['REMOVED']} | {c['UNVERIFIED']} | {c['MISSING']} | {n_changed} |")

# When did the fee in force take effect, for updated rows?
years = collections.Counter()
for r in upd:
    ys = re.findall(r"(?:19|20)\d\d", r[4])
    years[max(ys) if ys else "not stated"] += 1
print("\nupdated rows by year the fee in force took effect:", dict(sorted(years.items())))
