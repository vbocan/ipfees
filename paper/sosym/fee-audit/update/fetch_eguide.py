"""Fetch the PCT Applicant's Guide national chapter of each corpus office as plain text.

Usage: python fetch_eguide.py CORPUS_DIR OUT_DIR [CODE ...]

For every PCT-XX.ipf in CORPUS_DIR (or the codes given), downloads
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=XX&doc-lang=en and writes the visible
text, one block element per line, to OUT_DIR/XX.txt with the URL and retrieval date on top.
Standard library only.
"""
import datetime
import html
import pathlib
import re
import sys
import time
import urllib.request

URL = "https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code={code}&doc-lang=en"
BLOCK = r"</?(?:p|div|br|tr|li|h[1-6]|table|section)\b[^>]*>"


def to_text(page: str) -> str:
    page = re.sub(r"<(script|style)\b.*?</\1>", "", page, flags=re.S | re.I)
    page = re.sub(BLOCK, "\n", page, flags=re.I)
    page = re.sub(r"</t[dh]>", " | ", page, flags=re.I)
    page = html.unescape(re.sub(r"<[^>]+>", " ", page))
    lines = [re.sub(r"[ \t\r\f\v\xa0]+", " ", line).strip(" |") for line in page.split("\n")]
    return "\n".join(line for line in lines if line)


def main() -> None:
    corpus, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2])
    out.mkdir(parents=True, exist_ok=True)
    codes = sys.argv[3:] or sorted(p.stem[4:] for p in corpus.glob("PCT-*.ipf"))
    today = datetime.date.today().isoformat()
    for code in codes:
        url = URL.format(code=code)
        req = urllib.request.Request(url, headers={"User-Agent": "Mozilla/5.0 (fee audit)"})
        for attempt in range(3):
            try:
                with urllib.request.urlopen(req, timeout=60) as resp:
                    text = to_text(resp.read().decode("utf-8", errors="replace"))
                break
            except Exception as exc:  # noqa: BLE001 - report and retry
                print(f"{code}: attempt {attempt + 1} failed: {exc}", file=sys.stderr)
                time.sleep(5)
        else:
            continue
        (out / f"{code}.txt").write_text(f"Source: {url}\nRetrieved: {today}\n\n{text}\n", encoding="utf-8")
        first = text.split("\n", 1)[0]
        print(f"{code}: {len(text):>7} chars  {first[:90]}")
        time.sleep(1)


if __name__ == "__main__":
    main()
