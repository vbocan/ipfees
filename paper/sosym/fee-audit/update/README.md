# The corpus brought to the fees in force (October 2026)

All 118 schedules of the IPFees reference corpus were compared fee by fee with the official fees
in force on 20 October 2026 and edited to carry them. The baseline is IPFees `beab6fd`, whose
amounts are those of January 2024. This folder is the record of that pass, cited in Section 6.2 of
the paper.

## Method

1. `fetch_eguide.py` saved the WIPO PCT Applicant's Guide national chapter of every office as text
   (`sources/XX.txt`, 20 October 2026).
2. Ten AI research assistants (Claude), one per batch of eleven or twelve offices, followed
   `PROTOCOL.md`: they read each schedule, the saved chapter and, where it could be reached, the
   office's own fee table (saved with `fetch_page.py` as `sources/XX-office*.txt`), and edited the
   schedule's amounts, thresholds and conditions to the fees in force. They added no fee, input or
   reduction. Each wrote `batch-NN.md`, with the source quote behind every row.
3. The lead editor took the decisions below, built the corpus and ran every schedule through the
   checks on save (all 118 pass) and the IPFees test suites.
4. Further assistants, which had made none of the edits, checked every changed figure against
   the saved sources (`verify-A.md`, `verify-B.md`): 180 changed values, every one found in a saved
   source with the meaning the schedule gives it. The discrepancies they report are records that
   did not yet show the lead decisions; the records were then corrected.

`corpus-update.diff` is the batches' change to the corpus, against `beab6fd` (IPFees `1dbcecb`).
The lead's settlement of MN, MW and NA is IPFees `68b21e4`, and the version lines of the four
regional bases are `5965eb5`; the corpus as released is in the IPFees repository. `tally.py` counts the records.

## Lead decisions

- **Currency.** Bulgaria charges in euro since 1 January 2026, so PCT-BG now prices its fees in
  EUR at the amounts of the euro tariff (sources/BG-office-2.txt). Nicaragua's Ley 1025 (2020)
  sets the national fee at CA$240 for the first twenty claims; the Central American peso is at
  par with the US dollar (sources/NI-office-2.txt, secondary), so PCT-NI charges 240 USD. The
  Gambian and Zimbabwean schedules keep their currencies and their rows stay UNVERIFIED. The
  Malawian schedule keeps MWK and carries the local fee of the 2020 Regulations (MK 20,000;
  foreign applicants US$100), as the Gambian one carries the GMD fee.
- **Conditions wrong at the baseline.** The European and German renewal fees for the third year,
  and the Danish fees for the first three years, fall due at the second anniversary of the
  international filing date, not at 18 or 12 months; the US search fee was charged twice when the
  USPTO drew up the international preliminary report; Belize charged its first annuity on top of
  a filing fee that includes it. Each is corrected and marked "lead decision" in its record.
- **Directives.** Eight `VERIFY MONOTONIC` directives (BY, CU, KG, TM, US twice, UZ twice) named
  `ClaimCount` for fees that read another count, so they could never fail; each now names the
  count its fee reads, and IPFees refuses such a directive on save.
- **Labels.** The regional bases and the national schedules labelled `ApplicationDate` and
  `SheetCount` differently; the IPFees check on merged inputs found it, and the national schedules
  now use the bases' labels.
- **Readings accepted as the assistants chose.** PT 64.21 EUR (INPI's line for PCT national phase
  entry); VN keeps 150,000 VND (a halving to 31 December 2026 is reported only by a law firm); SK
  50 EUR and SV 50 USD; TR at the amounts of the 2026 chapter; KG and AE unchanged; no VAT added.
- **Mongolia, Malawi, Namibia.** The batch could confirm nothing in these three schedules. The
  lead settled each from the office's own text, saved in `sources/`: Mongolia's filing fee from
  the IPOM table under Government Resolution 151 of 2021 (35,000 MNT up to five claims, 5,000 per
  further claim); Malawi's specification fee from the Patents (Amendment) Regulations 2020, G.N.
  45/2020 (MK 20,000); Namibia's application fee from the Industrial Property Regulations 2018,
  confirmed by BIPA's page (N$1,000). Namibia's reported consolidation of 1 July 2026 (N$1,500,
  with registration and publication) rests on secondary sources only and is not applied.
- **Version lines.** Every schedule reads `VERSION '2026.10'` with the date from which its amounts
  apply.

## Totals (`python tally.py`)

| | Rows |
|---|---:|
| Fee lines checked | 455 |
| Match the fees in force | 298 (65 %) |
| Updated | 140 (31 %) |
| of which only an amount changed | 120 |
| of which a threshold, condition or basis changed | 20 (7 following an office's change since January 2024, 13 correcting the baseline) |
| Removed (the fee is no longer charged, or was charged twice) | 4 |
| Unverified (in 8 schedules) | 13 |
| Official fees or reductions not modelled | 105 |

48 schedules changed a fee, and every schedule changed its version line. A row is one
fee under one condition, with the entity-size variants of the same fee sharing a row; translation
and agent fees, which are placeholders, are not rows.

The fifteen-office audit of 19 October 2026 (`../AUDIT.md`) was the pilot of this pass; its
offices were checked again here.
