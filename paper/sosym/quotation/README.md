# Quotation measurements for Section 6 of the SoSyM paper

Run on 2026-10-20 against IPFees `68b21e4` (released as v1.1.0, whose application code and corpus
are those of `68b21e4`), built locally as containers under Docker Desktop 29.9 on an Intel Core
i9-13900K (32 logical processors, Windows 11). The corpus encodes the fees in force on 20 October 2026, and the
REST service starts from the compose set-up with no further settings. Earlier runs, against
`012d5a9` and before it, were superseded; this run replaces their files. Against `012d5a9`, whose
application code is the same, every amount agreed except the all-office mandatory total, which the
corrected Mongolian, Malawian and Namibian fees raise by 89 EUR. That run was on another machine (AMD
Ryzen 9 7900, 24 logical processors), and its times were 15–30 % higher. `measure.py` was run twice and the
second, warm run kept.

## Set-up

`compose.yml` starts the web front end (port 18080), the REST service (18090) and MongoDB. It
differs from the repository's compose file only in its ports and in setting the conversion markup to
zero. The web front end uses IPFees' own data folder: the fallback exchange-rate table shipped with
the application (166 currencies) and the default service-charge levels (100 / 175 / 250 EUR; every
office starts on the first).

```bash
git -C /path/to/ipfees archive v1.1.0 src | tar -x   # next to compose.yml
docker compose -f compose.yml up -d --build
python measure.py            # run it twice, keep the second; writes results.json, explain20.json, quote20.md, scaling.md
python split_optional.py     # Table 2: quote20_split.md
python probe_offices.py      # prices each office alone; all 118 answer
docker compose -f compose.yml down -v --rmi local
```

## Files

- `measure.py`: the twenty-office quotation, all served offices, the explanation with
  alternatives, and quote/explanation time against the number of offices (3 untimed and 30 timed
  sequential requests each, wall-clock at the client).
- `results.json`, `quote20.md`, `scaling.md`, `explain20.json`: its output (Section 6.1 quotes the
  twenty-office and all-office times and the slope of the scaling series, about 1.7 ms per office).
- `split_optional.py`, `quote20_split.md`: Table 2, with each office's optional amount split into
  elective official fees and the corpus's translation and agent placeholders.
- `probe_offices.py`: prices each office alone; all 118 answer.

The screenshots of the quotation (Figs. 3, 5, 7 and 8) were taken from the same set-up at `012d5a9`,
whose application code and twenty-office amounts are those of v1.1.0.
