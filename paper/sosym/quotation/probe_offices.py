"""Price the running example in every office on its own and list the offices the API refuses.

Usage: python probe_offices.py [api_base]
"""
import json
import sys
from collections import Counter

sys.argv = sys.argv[:2]
import measure  # noqa: E402  (reuses the inputs and the HTTP helper)

if len(sys.argv) > 1:
    measure.API = sys.argv[1]

_, jurisdictions, _ = measure.call('GET', '/Jurisdiction')
failed = {}
for j in jurisdictions:
    code = j['jurisdiction']
    status, payload, _ = measure.call('POST', '/Fee/Calculate', measure.request([code]))
    if status != 200:
        _, explained, _ = measure.call('POST', '/Fee/Explain', measure.request([code]))
        failed[code] = (status, str(explained)[:300])
print(f'{len(jurisdictions) - len(failed)} of {len(jurisdictions)} offices answer; {len(failed)} fail')
print(Counter(s for s, _ in failed.values()))
for code, (status, why) in sorted(failed.items())[:12]:
    print(code, status, why)
