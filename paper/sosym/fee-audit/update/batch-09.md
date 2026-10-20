# Batch 09: SE SG SK SV SY TH TJ TM TN TR TT TZ

Checked on 20 October 2026 against the saved eGuide chapters (`sources/XX.txt`, all retrieved
2026-10-20) and, where an office table could be reached and read, against that table. Baseline:
IPFees `beab6fd` (`VERSION '2024.1' EFFECTIVE 2024-01-01` in every file). Only the twelve
schedules of this batch were edited; translation and agent fees were not touched.

## SE: PCT-SE.ipf (SEK)

Sources: eGuide SE, valid as from August 1, 2026 (sources/SE.txt); PRV "Fees and payment" page, page dated "Edited 2026-01-01" (sources/SE-office-2.txt); PRV combined fee list "Gäller från och med 15 oktober 2023" (sources/SE-office.txt, superseded, kept for the record).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-08-01 (the chapter's "Valid as from" date; neither PRV page states an effective date for the amounts modelled).
Fees last changed: not stated for the fees modelled (the PRV page gives 1 December 2025 only for the grant fee, which is not modelled).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 500 | 500 | not stated | MATCH | SE "Entry fee 500 SEK"; SE-office-2 "Application fee: 500 kr." |
| OFF_SearchFee | always | 2500 | 2500 | not stated | MATCH | SE "Search fee 2,500 SEK"; SE-office-2 "Search fee: 2500 kr." |
| OFF_ClaimFee | per claim above 10 | 150 | 150 | not stated | MATCH | SE "Claim fee for each claim in excess of 10 150 SEK"; SE-office-2 "beyond the first ten ... 150 kr" |
| OFF_Renewal3 | 3rd year, from 24 months | 1500 | 1600 | not stated | UPDATED (amount) | SE "Annual fee for the 3rd year ... 1,600 SEK"; SE-office-2 "N-3 3rd annual fee Yes 1 600 kr" |

Notes: the PRV list of 15 October 2023 (SE-office.txt) still shows "3:e avgiftsåret 1 500"; the
PRV fee page and the August 2026 chapter both give 1,600, so the 3rd-year fee rose after the
baseline, on a date neither source states. The annual fee is taken from the PRV page and the
chapter, the more recent sources.

## SG: PCT-SG.ipf (SGD)

Sources: eGuide SG, valid as from June 1, 2026 (sources/SG.txt); IPOS forms and fees page, "Last updated on 9 October 2026" (sources/SG-office.txt); ../AUDIT.md, batch 3, SG (19 October 2026).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-09-01 (the effective date the office states for its current table: "the majority of the fee updates will take effect on 1 September 2025").
Fees last changed: 1 September 2025 (other updates 1 April 2026, none of them modelled here).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (PF37) | 200 | 210 | 1 Sep 2025 (office's general date; not stated per fee) | UPDATED (amount) | SG "Patents Form 37 — Entry into national phase 210 SGD"; SG-office "PF37 Entry into National Phase S$210" |
| OFF_ExaminationFee | base fee (PF12) | 1350 | 1420 | 1 Sep 2025 | UPDATED (amount) | SG "Patents Form 12 — Request for an examination report 1,420 SGD"; SG-office "S$1,420 plus S$80 for each claim over 15 claims" |
| OFF_ExaminationFee | per claim above threshold | 40 per claim above 20 | 80 per claim above 15 | 1 Sep 2025 | UPDATED (amount, threshold) | SG "— plus for each claim in excess of 15 claims 80 SGD"; SG-office "S$80 for each claim over 15 claims" |
| PF11 combined search and examination | IPOS established the ISR/IPRP / other cases | — | 1,750 / 2,050 + 80 per claim above 15 | 1 Sep 2025 | MISSING | SG "(a) ... established ... by the Office under the PCT 1,750 SGD", "(b) In any other case 2,050 SGD" |

Notes: confirms AUDIT.md in every figure. The excess-claim charge now starts at the 16th claim
(`IF ClaimCount GT 15`); the fee has no explicit zero case, as at baseline, since its base fee is
always due. Not modelled and not national-phase-entry fees: PF13A (SGD 80 per claim added by a
response, from 1 April 2026) and the flat SGD 210 grant fee (PF14) for requests filed on or after
1 September 2025.

## SK: PCT-SK.ipf (EUR)

Sources: eGuide SK, valid as from June 11, 2026, "Summary of latest changes in this version: Annexes: National fees" (sources/SK.txt); ÚPV SR schedule of administrative fees 2025, item 216 (sources/SK-office-2.txt, PDF SpravnePoplatky2025); ÚPV SR English page "Forms and fees for new applications", undated and out of date (sources/SK-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-11 (the chapter's "Valid as from" date; the office's schedule states no effective date).
Fees last changed: not stated (the office's 2025 schedule already carries the current amounts).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | applicant other than the inventor, electronic filing | 60 | 50 | not stated | UPDATED (amount, basis) | SK "applicant other than the inventor ... 100 EUR"; "reduced by 50% where the application is filed by electronic means, however ... by a maximum of 50 EUR"; SK-office-2 "216 a) ... 2. iným prihlasovateľom ako pôvodcom ... 100 €" |
| OFF_ExaminationFee | base fee, up to 10 claims | 116 | 170 | not stated | UPDATED (amount) | SK "Fee for requesting examination: — basic fee 170 EUR"; SK-office-2 "1. do 10 uplatnených patentových nárokov 170 €" |
| OFF_ExaminationFee | per claim above 10 | 20 | 30 | not stated | UPDATED (amount) | SK "additional fee for each claim in excess of 10 30 EUR"; SK-office-2 "2. za každý ďalší uplatnený patentový nárok 30 €" |
| Filing fee, inventor(s) as applicant | applicant is the inventor | — | 50 (25 electronic) | not stated | MISSING | SK "filed by an inventor or co-inventors 50 EUR" |
| Examination fee halved | ITS by the Office as VPI branch, or search in the priority period | — | 50 % of the fee | not stated | MISSING | SK-office-2 "Úrad vyberie poplatok podľa písmena c) ... vo výške polovice príslušnej sadzby" |

Notes: the baseline 60 and 116 are the full (non-electronic) amounts of the superseded table
still shown on the office's English page (SK-office.txt: "applicant other than the originator
... 60", "complete examination ... 116"). The filing fee is now 100, or 50 when filed
electronically (50 % off, capped at EUR 50); the schedule carries the electronic rate, as the
protocol asks. The 50 is derived from the two quoted statements, not printed as a separate figure.
Whether the halved examination fee applies to a PCT application whose ISR was drawn up by the
Visegrad Patent Institute is not clear from the note; recorded as MISSING with that doubt.
`OFF_ExaminationFee` has no explicit zero case, as at baseline.

## SV: PCT-SV.ipf (USD)

Sources: eGuide SV, valid as from January 9, 2026 (sources/SV.txt). Office table not reachable: the CNR service catalogue lists services without amounts and the CNR fee page (cnr.gob.sv/aranceles-registro-de-la-propiedad-intelectual/) returns 404; neither was kept.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-09 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 57.14 | 50 | not stated | UPDATED (amount) | SV "Filing fee 50 USD" |

Notes: 57.14 equals 500 colones at the fixed rate of 8.75 colones per dollar, which suggests the
baseline came from the statutory tariff in colones (an inference; no saved source states it). The
chapter, the only saved source and later than the baseline, gives USD 50. Applied under the
protocol, but worth a check against the CNR tariff if it can be reached.

## SY: PCT-SY.ipf (SYP)

Sources: eGuide SY, valid as from October 15, 2026 (sources/SY.txt). Annex SY.I: "Information not yet available"; no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-10-15 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | other / natural person | 5000 / 250 | 5000 / 250 | not stated | MATCH | SY "Filing fee: ... 5,000 SYP (250 SYP)" |
| OFF_SheetFee | per page above 30, other / natural person | 50 / 10 | 50 / 10 | not stated | MATCH | SY "each page in excess of 30: 50 SYP (10 SYP)" |
| OFF_ClaimFee | per claim above 10, other / natural person | 500 / 250 | 500 / 250 | not stated | MATCH | SY "claims in excess of 10 ... 500 SYP (250 SYP)" |
| OFF_ExaminationFee | other / natural person | 10000 / 1000 | 20000 / 2000 | not stated | UPDATED (amount) | SY "Examination fee: 20,000 SYP (2,000 SYP)" |
| OFF_Renewal | 2nd year, other / natural person | 4500 / 250 | 5500 / 300 | not stated | UPDATED (amount) | SY "— for the 2nd year: 5,500 SYP (300 SYP)" |
| OFF_Renewal | 3rd year, other / natural person | 5000 / 300 | 6000 / 350 | not stated | UPDATED (amount) | SY "— for the 3rd year: 6,000 SYP (350 SYP)" |
| Annual fee reductions | students and microenterprises / small enterprises | — | 90 % / 50 % off | not stated | MISSING | SY "reduced by 90% for students and microenterprises ... reduced by 50% for small enterprises" |

Notes: the chapter says "The amount in parentheses is applicable in case of filing by a natural
person", which the schedule maps to `Entity_Person`. The chapter also says "Exemptions,
reductions or refunds of fees: None", contradicting the annual-fee reductions it lists two
paragraphs earlier.

## TH: PCT-TH.ipf (THB)

Sources: eGuide TH, valid as from August 15, 2024 (sources/TH.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2024-08-15 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 500 | 500 | not stated | MATCH | TH "Patents Filing fee 500 THB" |
| OFF_PublicationFee | optional | 250 | 250 | not stated | MATCH | TH Annex TH.I "Publication fee 250 THB" |
| OFF_ExaminationFee | optional | 250 | 250 | not stated | MATCH | TH Annex TH.I "Request for examination fee 250 THB" |

Notes: the chapter is the only source and dates from August 2024. A web search found reports of a
draft revision of the Thai Patent Act with higher fees, but no saved source shows it enacted;
nothing applied.

## TJ: PCT-TJ.ipf (USD)

Sources: eGuide TJ, valid as from January 1, 2026 (sources/TJ.txt). Annex TJ.I: "Information not yet available".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | one invention | 50 | 50 | not stated | MATCH | TJ "Filing fee: ... — for one invention 50 USD" |
| OFF_BasicNationalFee | each invention above one | 20 | 20 | not stated | MATCH | TJ "— for each invention in excess of one 20 USD" |
| OFF_ExaminationFee | one invention | 410 | 410 | not stated | MATCH | TJ "Examination fee: — for one invention 410 USD" |
| OFF_ExaminationFee | each invention above one | 330 | 330 | not stated | MATCH | TJ "— for each invention in excess of one 330 USD" |
| OFF_Renewal3 | 3rd year, from 24 months | 50 | 50 | not stated | MATCH | TJ "Annual fee for the 3rd year 50 USD" |
| Examination fee reduction | ISR or IPER established | — | 20 % off | not stated | MISSING | TJ "The examination fee is reduced by 20% where an international search or an international preliminary examination report has been established" |

Notes: the schedule counts inventions as `IndependentClaimCount`, a simplification kept as is.

## TM: PCT-TM.ipf (USD)

Sources: eGuide TM, valid as from July 30, 2026 (sources/TM.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-30 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (one independent claim) | 100 | 100 | not stated | MATCH | TM "Filing fee (includes publication and limited examination for one independent claim) ... 100 USD" |
| OFF_ClaimFee | per independent claim above one | 25 | 30 | not stated | UPDATED (amount) | TM "Claim fee for each independent claim in excess of one ... 30 USD" |
| OFF_Renewal1 | 1st year | 50 | 50 | not stated | MATCH | TM "Maintenance fee for patent (limited patent), for the 1st year 50 USD" |
| Reduction for resident individuals | applicant is an individual permanently residing in Turkmenistan | — | 90 % off | not stated | MISSING | TM "Fees are reduced by 90% for applicants who are individuals permanently residing in Turkmenistan" |

Notes: payment is in US dollars for foreign applicants (manats only for residents), so the
currency holds. Baseline doubt, not changed: `VERIFY MONOTONIC FEE OFF_ClaimFee WITH RESPECT TO
ClaimCount` while the fee depends on `IndependentClaimCount`. The fee for a request for patent
examination (USD 120, converting a limited patent) is not a national-phase-entry fee and is not
counted.

## TN: PCT-TN.ipf (TND)

Sources: eGuide TN, valid as from March 19, 2026 (sources/TN.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-03-19 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (filing fee and 1st annual fee) | 140 | 140 | not stated | MATCH | TN "Filing fee and 1st annual fee 140 TND" |
| OFF_ClaimFee | per claim above 10 | 30 | 30 | not stated | MATCH | TN "Claim fee for each claim in excess of 10 per claim 30 TND" |
| OFF_Renewal | 2nd year, from 12 months | 50 | 50 | not stated | MATCH | TN "from the 2nd to the 5th year per year 50 TND" |
| OFF_Renewal | 3rd year, from 24 months | 50 | 50 | not stated | MATCH | TN "from the 2nd to the 5th year per year 50 TND" |

Notes: amounts exclude VAT ("Fees are subject to value added tax (VAT) of 19%"), as at baseline.

## TR: PCT-TR.ipf (TRY)

Sources: eGuide TR, valid as from April 10, 2026, last updated September 21, 2026, "With effect since 1 January 2026: ... National fees" (sources/TR.txt). The office's tariff (Resmi Gazete 31 December 2025, BİK/TÜRKPATENT 2026/1, 20251231M5-37.pdf) was downloaded but its text cannot be extracted (font encoding); not kept and not used. TÜRKPATENT's own fee pages returned 404.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the date the chapter states for its national fees).
Fees last changed: 1 January 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 2015 | 16890 | 1 Jan 2026 | UPDATED (amount) | TR "Filing fee — for patents 16,890 TRY" |
| OFF_SearchFee | no ISR; other / natural person | 805 / 400 | 5180 / 2530 | 1 Jan 2026 | UPDATED (amount) | TR "Fee for requesting search report (i) Up to 10 applications for natural persons, up to 100 applications for universities 2,530 TRY (ii) ... 5,180 TRY" |
| OFF_Renewal3 | 3rd year, from 24 months | 400 | 3800 | 1 Jan 2026 | UPDATED (amount) | TR "Renewal fee for the 3rd year — for patents 3,800 TRY" |
| Examination fee | always (request within 3 months of the search report) / natural person | — | 5,180 / 2,530 | 1 Jan 2026 | MISSING | TR "Fee for requesting examination report: (i) ... 2,530 TRY (ii) ... 5,180 TRY" |

Notes: the reduced search rate applies to "Up to 10 applications for natural persons, up to 100
applications for universities"; the schedule keeps its baseline mapping to `Entity_Person` only.
The 2nd-year fee (2,636.80) applies only to applications that entered the national phase before
10 January 2017 and is not modelled. The jump from the baseline amounts is large (about 8x), so
the baseline may already have been out of date at 1 January 2024; the saved sources give no
earlier figure to confirm it.

## TT: PCT-TT.ipf (TTD)

Sources: eGuide TT, valid as from May 21, 2026 (sources/TT.txt); TTIPO Patent Application Manual, undated (sources/TT-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-21 (the chapter's "Valid as from" date; the manual is undated).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 2000 | 2000 | not stated | MATCH | TT "Filing fee ... 2,000 TTD"; TT-office "Application for a patent ... $2,000.00 TT" |
| OFF_SearchFee | always (search and substantive examination) | 1500 | 1500 | not stated | MATCH | TT "Fee for search and substantive examination 1,500 TTD"; TT-office "$1,500.00 TT" |
| OFF_Renewal | 2nd year, from 12 months | 200 | 200 | not stated | MATCH | TT Annex "— for the 2nd year 200 TTD"; TT-office "2nd year $200.00 TT" |
| OFF_Renewal | 3rd year, from 24 months | 400 | 400 | not stated | MATCH | TT Annex "— for the 3rd year 400 TTD"; TT-office "3rd year $400.00 TT" |

Notes: the chapter's summary gives "Annual fee for the first three years 1,000 TTD", which
contradicts its own Annex TT.I (200 + 400) and the office manual; the annex and the manual agree,
and the schedule follows them.

## TZ: PCT-TZ.ipf (TZS)

Sources: eGuide TZ, valid as from March 20, 2025 (sources/TZ.txt). Annex TZ.I: "Information not yet available".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-20 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 12000 | 12000 | not stated | MATCH | TZ "Patents Filing fee 12,000 TZS" |

Notes: none.

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| SE | 4 | 3 | 1 | 0 | 0 | 0 | 2026-08-01 (chapter) | not stated |
| SG | 4 | 0 | 3 | 0 | 0 | 1 | 2025-09-01 (office) | 1 Sep 2025 |
| SK | 5 | 0 | 3 | 0 | 0 | 2 | 2026-06-11 (chapter) | not stated |
| SV | 1 | 0 | 1 | 0 | 0 | 0 | 2026-01-09 (chapter) | not stated |
| SY | 7 | 3 | 3 | 0 | 0 | 1 | 2026-10-15 (chapter) | not stated |
| TH | 3 | 3 | 0 | 0 | 0 | 0 | 2024-08-15 (chapter) | not stated |
| TJ | 6 | 5 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | not stated |
| TM | 4 | 2 | 1 | 0 | 0 | 1 | 2026-07-30 (chapter) | not stated |
| TN | 4 | 4 | 0 | 0 | 0 | 0 | 2026-03-19 (chapter) | not stated |
| TR | 4 | 0 | 3 | 0 | 0 | 1 | 2026-01-01 (chapter, stated effect) | 1 Jan 2026 |
| TT | 4 | 4 | 0 | 0 | 0 | 0 | 2026-05-21 (chapter) | not stated |
| TZ | 1 | 1 | 0 | 0 | 0 | 0 | 2025-03-20 (chapter) | not stated |
| **All** | **47** | **25** | **15** | **0** | **0** | **7** | | |
