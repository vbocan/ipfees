"""Save an office's fee page as plain text, with its URL and retrieval date on top.

Usage: python fetch_page.py URL OUT.txt

The page is downloaded with curl (which uses the system's certificate store; several office
sites send incomplete chains that Python's own store rejects). HTML is reduced to its visible
text, one block element per line and table cells separated by " | ". A PDF is saved next to
OUT.txt as OUT.pdf, and its text is extracted with poppler's pdftotext in Docker
(minidocks/poppler).
"""
import datetime
import os
import pathlib
import subprocess
import sys

from fetch_eguide import to_text


def main() -> None:
    url, out = sys.argv[1], pathlib.Path(sys.argv[2])
    out.parent.mkdir(parents=True, exist_ok=True)
    raw = out.with_suffix(".download")
    subprocess.run(["curl", "-sSL", "--max-time", "120", "-A", "Mozilla/5.0 (fee audit)",
                    "-o", str(raw), url], check=True)
    body = raw.read_bytes()
    header = f"Source: {url}\nRetrieved: {datetime.date.today().isoformat()}\n\n"
    if body[:5] == b"%PDF-":
        pdf = out.with_suffix(".pdf")
        raw.replace(pdf)
        env = dict(os.environ, MSYS_NO_PATHCONV="1")
        subprocess.run(
            ["docker", "run", "--rm", "-v", f"{pdf.parent.resolve()}:/w", "-w", "/w",
             "minidocks/poppler", "pdftotext", "-layout", pdf.name, out.name],
            check=True, env=env)
        out.write_text(header + out.read_text(encoding="utf-8", errors="replace"), encoding="utf-8")
    else:
        raw.unlink()
        out.write_text(header + to_text(body.decode("utf-8", errors="replace")) + "\n", encoding="utf-8")
    print(f"{out}: {out.stat().st_size} bytes")


if __name__ == "__main__":
    main()
