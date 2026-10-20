# Batch 10: EP UA UG US UZ VC VN WS ZA ZM ZW

Checked on 20 October 2026 against the saved eGuide chapters (`sources/XX.txt`) and, for EP and US,
the office tables already recorded in `../AUDIT.md` and saved again here (`sources/EP-office.txt`,
`sources/EP-office-2.txt`, `sources/US-office.txt`). For the other nine offices the eGuide chapter
(applicable from 2025 or 2026) was the source; the office sites were not consulted except where the
chapter looked stale (VN, ZW), and there only secondary sources could be found
(`sources/VN-office.txt`, `sources/ZW-office.txt`, both law-firm pages, not office tables).
Only the eleven schedules named above were edited.

## EP: PCT-EP.ipf (EUR)

Sources: eGuide EP, valid as from 17 September 2026 (sources/EP.txt); Rules relating to Fees, consolidated version "applicable as from 1 April 2026" (sources/EP-office.txt, PDF saved alongside); decision CA/D 9/25, OJ EPO 2026, A2 (sources/EP-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-01 (effective date of the office's current table: OJ EPO 2026, A2, Art. 4 "the new amounts ... apply to payments made on or after 1 April 2026").
Fees last changed: 1 April 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee (filing fee) | Form 1200 filed online | 135 | 135 | — | MATCH | EP-office-2 "is filed online 135"; EP "for online filings 135 EUR" |
| OFF_DesignationFee | always | 660 | 720 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 3 "720"; EP "Designation fee ... 720 EUR" |
| OFF_ExtensionFee (BA) | Extension requested | 102 | 102 | — | MATCH | EP "Bosnia and Herzegovina ... 102 EUR" |
| OFF_SheetFee | each page > 35 | 17 | 17 | — | MATCH | EP-office-2 item 1a "plus EUR 17 for the 36th"; EP "17 EUR" |
| OFF_ClaimFee | 16th–50th claim | 265 | 290 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 15 "up to the limit of 50 290" |
| OFF_ClaimFee | 51st claim on | 660 | 720 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 15 "51st and each subsequent claim 720" |
| OFF_SearchFee | ISA other than EPO and the seven below | 1,460 | 1,595 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 2 "supplementary European search ... 1 595" |
| OFF_SearchFee | ISA AT/ES/FI/SE/TR/XN/XV | 275 (1,460 − 1,185) | 230 (1,595 − 1,365) | 1 Apr 2026 | UPDATED (amount) | EP-office-2 Art. 3 "is reduced by EUR 1 365"; EP "reduced by 1,365 EUR" |
| OFF_SearchFee | ISA = EPO | 0 | 0 | — | MATCH | EP "No search fee is payable: — where the international search report has been established by the EPO" |
| OFF_ExaminationFee | no supplementary search (ISA = EPO) | 2,055 | 2,240 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 6 "no supplementary European search report ... 2 240" |
| OFF_ExaminationFee | supplementary search drawn up | 1,840 | 2,010 | 1 Apr 2026 | UPDATED (amount) | EP-office-2 item 6 "supplementary European search report is drawn up ... 2 010" |
| OFF_ExaminationFee | IPER by EPO: fee × 0.25 | × 0.25 | reduced by 75 % | — | MATCH | EP "reduced by 75% where the international preliminary report ... established by the EPO" |
| OFF_ValidationFee_KH | selected | 180 | 180 | — | MATCH | EP "Cambodia ... 180 EUR" |
| OFF_ValidationFee_MA | selected | 240 | 240 | — | MATCH | EP "Morocco ... 240 EUR" |
| OFF_ValidationFee_MD | selected | 200 | 200 | — | MATCH (see notes: now only for IAs filed before 1 June 2026) | EP "Republic of Moldova ... filed prior to 1 June 2026 ... 200 EUR" |
| OFF_ValidationFee_TN | selected | 180 | 180 | — | MATCH | EP "Tunisia ... 180 EUR" |
| OFF_Renewal3 | ≥ 18 months from filing date | 530 | 725 | 1 Apr 2026 | UPDATED (amount, condition: due at 24 months; lead decision, README.md) | EP-office-2 item 4 "for the 3rd year 725"; EP "Renewal fee for the 3rd year ... 725 EUR" |
| Filing fee, not online | Form 1200 not filed online | — | 285 | — | MISSING | EP-office-2 "is not filed online 285" |
| Validation fee, Georgia | IA filed on or after 15 Jan 2024 | — | 200 | — | MISSING | EP "Georgia ... 200 EUR" |
| Validation fee, Lao PDR | IA filed on or after 1 Apr 2025 | — | 180 | — | MISSING | EP "Lao People’s Democratic Republic ... 180 EUR" |
| 30 % reduction, Rule 7a EPC | micro-enterprises, natural persons, non-profits, universities, public research organisations | — | 30 % | — | MISSING | EP "may be entitled to a 30% fee reduction on the most common fees" |
| 30 % language reduction (examination fee) | "in certain cases" | — | 30 % | — | MISSING | EP "reduced by 30% for language reasons" |

Notes:
- The OJ EPO 2026, A2 reduction of the supplementary search fee applies "to international applications filed up to and including 31 March 2028 ... for which the fee for the supplementary European search is paid on or after 1 April 2026" (EP-office-2, Art. 4). The schedule applies it without the filing-date limit; harmless today, wrong for IAs filed from 1 April 2028.
- Moldova: the eGuide now says validation is "only available for international applications filed prior to 1 June 2026" (OJ EPO 2026, A25). The schedule offers MD without a date condition. Adding one would need a date comparison on `ApplicationDate` inside the validation fee; not done (see final report). The other validation states also have filing-date thresholds (KH from 1 Mar 2018, MA from 1 Mar 2015, TN from 1 Dec 2017) that the baseline does not model.
- OFF_Renewal3 condition left unchanged at `RenMonth GTE 18`. The eGuide says the fee is due by the end of the month of the second anniversary (24 months) of the international filing date, or at 31 months from priority if later. 18 months may be meant as the earliest date of valid payment (six months before due); this cannot be confirmed from the saved sources, so the threshold was not changed. The fee has no explicit zero case and no VERIFY directive at baseline.
- Effective dates: the in-force amounts apply from 1 April 2026. `../AUDIT.md` reports third-party pre-April-2026 values (685 designation, 1,520 search, 1,915/2,135 examination). On those figures the baseline amounts were already superseded on 1 April 2024. That round is not in a saved source.

## US: PCT-US.ipf (USD; large / small / micro)

Sources: eGuide US, valid as from 20 August 2026 (sources/US.txt); USPTO fee schedule, "Effective January 19, 2025 (Last revised August 14, 2026)" (sources/US-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-01-19 (effective date of the office's current table).
Fees last changed: 19 January 2025 for the national-stage fees (the 14 August 2026 revision concerns petition fees only).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 320/128/64 | 350/140/70 | 19 Jan 2025 | UPDATED (amount) | US-office "1631/2631/3631 ... Basic national stage fee \| 350.00 \| 140.00 \| 70.00" |
| OFF_SearchFee | IPRP by US | 0 | 0 | — | MATCH (condition broader than the rule, see notes) | US-office "1.492(b)(1) ... \| 0.00" |
| OFF_SearchFee | ISA = US | 140/56/28 | 150/60/30 | 19 Jan 2025 | UPDATED (amount, condition: not when the US drew up the IPRP; lead decision, README.md) | US-office "1641 ... U.S. was the ISA \| 150.00 \| 60.00 \| 30.00" |
| OFF_SearchFee | ISR by another ISA | 540/216/108 | 580/232/116 | 19 Jan 2025 | UPDATED (amount) | US-office "1642 ... search report prepared and provided to USPTO \| 580.00 \| 232.00 \| 116.00" |
| OFF_SearchFee | no ISR | 700/280/140 | 770/308/154 | 19 Jan 2025 | UPDATED (amount, condition: not when the US drew up the IPRP; lead decision, README.md) | US-office "1632 ... all other situations \| 770.00 \| 308.00 \| 154.00" |
| OFF_ExaminationFee | IPRP by US | 0 | 0 | — | MATCH (condition broader than the rule) | US-office "1.492(c)(1) ... \| 0.00" |
| OFF_ExaminationFee | otherwise | 800/320/160 | 880/352/176 | 19 Jan 2025 | UPDATED (amount) | US-office "1633 ... \| 880.00 \| 352.00 \| 176.00" |
| OFF_SheetFee | each 50 sheets > 100 | 420/168/84 | 450/180/90 | 19 Jan 2025 | UPDATED (amount) | US-office "1681 ... exceeds 100 sheets \| 450.00 \| 180.00 \| 90.00" |
| OFF_IndependentClaimFee | each > 3 | 480/192/96 | 600/240/120 | 19 Jan 2025 | UPDATED (amount) | US-office "1614 ... in excess of three \| 600.00 \| 240.00 \| 120.00" |
| OFF_ClaimFee | each > 20 | 100/40/20 | 200/80/40 | 19 Jan 2025 | UPDATED (amount) | US-office "1615 ... in excess of 20 \| 200.00 \| 80.00 \| 40.00" |
| OFF_MultipleDependentClaimFee | any MDC, per application | 860/344/172 | 925/370/185 | 19 Jan 2025 | UPDATED (amount) | US-office "1616 ... Multiple dependent claim \| 925.00 \| 370.00 \| 185.00"; US "per application" |
| Surcharge 1617 | search/exam fee or oath after commencement | — | 170/68/34 | — | MISSING | US-office "1617 ... \| 170.00 \| 68.00 \| 34.00" |
| Processing fee 1618 | English translation after 30 months | — | 150/60/30 | — | MISSING | US-office "1618 ... \| 150.00 \| 60.00 \| 30.00" |

Notes:
- All eleven amounts agree between US-office and the eGuide.
- The fees in force since 19 January 2025 replaced the baseline; the baseline was current on 1 January 2024.
- Baseline conditions not changed: the zero search and examination fees apply under 37 CFR 1.492(b)(1)/(c)(1) only when "U.S. was the ISA or IPEA and all claims satisfy PCT Article 33(1)-(4)". The schedule grants them for any IPRP by the US. Also, `OFF_SearchFee` sums its CASE blocks, so with IPRP = US and ISA = US it yields 0 + 150, not 0 (the CASE blocks are not exclusive).
- Mega-sequence-listing fees (1,140 / 11,290 large entity) are not modelled; not counted as MISSING because they apply only to sequence listings of 300 MB and more.

## UA: PCT-UA.ipf (UAH)

Sources: eGuide UA, valid as from 1 January 2026 (sources/UA.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | online filing | 1,280 | 1,280 | — | MATCH | UA "online 1,280 UAH or equivalent in EUR or USD" |
| OFF_SheetFee | each 50 sheets > 150, online | 640 | 640 | — | MATCH | UA "each 50 sheets in excess of 150: online 640 UAH" |
| OFF_ClaimFee | each claim > 3 | 160 | 160 | — | MATCH | UA "in excess of three: 160 UAH" |
| OFF_ExaminationFee | 6,000 × independent claims | 6,000 | 6,000 + 6,000 per independent claim > 1 | — | MATCH | UA "Examination fee ... 6,000 UAH"; "each independent claim in excess of one: 6,000 UAH" |
| Reduction for applicant-inventors / non-profits | all applicants inventors / non-profit | — | 90 % / 80 % | — | MISSING | UA "All fees are reduced by 90% where all applicants are also inventors and by 80% ..." |

Notes: paper-filing rates (1,600; 800) exist; the schedule keeps the online rate. The examination fee is due within three years from the international filing date; the schedule models it as OPTIONAL.

## UG: PCT-UG.ipf (USD; inherits bases/base_ap.ipf)

Sources: eGuide UG, valid as from 1 January 2026, last updated 15 September 2026 (sources/UG.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 150 | 150 | — | MATCH | UG "National processing fee 60,000 UGX 150 USD" |
| OFF_PublicationFee | always | 50 | 50 | — | MATCH | UG "Publication fee 50,000 UGX 50 USD" |
| OFF_ExaminationFee | optional | 250 | 250 | — | MATCH | UG Annex UG.I "Request for examination 150,000 UGX 250 USD" |
| OFF_Renewal | 2nd year, ≥ 12 months from filing | 50 | 50 | — | MATCH | UG "Annual fee for the 2nd year ... 50,000 UGX 50 USD" |
| OFF_Renewal | 3rd year, ≥ 24 months from filing | 70 | 70 | — | MATCH | UG Annex UG.I "for the 3rd year 70,000 UGX 70 USD" |
| Grant fee | listed among the national fees due at entry | — | 100 | — | MISSING | UG "Grant fee 90,000 UGX 100 USD" (under "National fees: Must be furnished or paid within the time limit applicable under PCT Article 22 or 39(1)") |

Notes: foreign applicants pay in USD ("must effect payment in US dollar"), consistent with the schedule's currency. The base file was not touched.

## UZ: PCT-UZ.ipf (USD)

Sources: eGuide UZ, valid as from 1 January 2026 (sources/UZ.txt). Annex UZ.I: "Information not yet available".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 420 | 420 | — | MATCH | UZ "Filing fee ... 420 USD" |
| OFF_IndependentClaimFee | each independent claim > 1 | 210 | 210 | — | MATCH | UZ "each independent claim in excess of one ... 210 USD" |
| OFF_DependentClaimFee | dependent claims 11–20 | 84 | 84 | — | MATCH (reading ambiguous, see notes) | UZ "each dependent claim in excess of 10 ... 84 USD" |
| OFF_DependentClaimFee | dependent claims > 20 | 42 | 42 | — | MATCH (reading ambiguous) | UZ "each dependent claim in excess of 20 ... 42 USD" |
| OFF_Renewal | always | 840 | 840 | — | MATCH | UZ "Maintenance fee for the first three years 840 USD" |
| Examination fee | 1,260 + 1,260 per independent claim > 1 | — | 1,260 | — | MISSING | UZ "Examination fee 1,260 USD"; "each independent claim in excess of one 1,260 USD" |
| Examination fee reduction | ISR or IPER established | — | 1,000 + 840 per independent claim > 1 | — | MISSING | UZ "The examination fee is reduced ... Examination fee 1,000 USD ... 840 USD" |

Notes: the chapter does not say when the examination fee is due, unlike the filing and claim fees ("Must be furnished or paid within the time limit ..."). The two dependent-claim lines could also mean 84 + 42 per claim beyond 20. The schedule's reading (84 for claims 11–20, 42 beyond) was kept.

## VC: PCT-VC.ipf (XCD)

Sources: eGuide VC, valid as from 20 March 2025 (sources/VC.txt). Annex VC.I: "Information not yet available".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-20 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 1,000 | 1,000 | — | MATCH | VC "National processing fee 1,000 XCD" |
| Annual fee, 4th year | due at entry | — | 200 | — | MISSING | VC "Annual fee for the 4th year 200 XCD" (under "National fees: Must be furnished or paid within the time limit ...") |

## VN: PCT-VN.ipf (VND)

Sources: eGuide VN, valid as from 28 May 2026 (sources/VN.txt); law-firm summary of Circular 64/2025/TT-BTC (sources/VN-office.txt; secondary, not the office's table).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-28 (chapter "Valid as from" date).
Fees last changed: not stated by the office; see notes on Circular 64/2025/TT-BTC.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee (filing fee) | always | 150,000 | 150,000 (eGuide) or 75,000 (secondary source) | — | UNVERIFIED: the eGuide gives 150,000, but a law-firm page reports the filing fee halved to 75,000 from 1 Jul 2025 to 31 Dec 2026 under Circular 64/2025/TT-BTC. Official text not obtained | VN "Filing fee ... 150,000 VND"; VN-office "Lệ phí nộp đơn ...: 75.000 đồng" |
| OFF_SearchFee | always | 600,000 | 600,000 | — | MATCH | VN Annex VN.I "Search fee 600,000 VND" |
| OFF_PriorityFee | per priority | 600,000 | 600,000 | — | MATCH | VN "Fee for priority claims per priority 600,000 VND" |
| OFF_FormalExaminationFee | always | 180,000 | 180,000 | — | MATCH | VN Annex VN.I "Fee for formality examination 180.000 VND" |
| OFF_FormalExaminationFee | each sheet > 6 | 8,000 | 8,000 | — | MATCH | VN Annex VN.I "each sheet in excess of six 8,000 VND" |
| OFF_ExaminationFee | always | 720,000 | 720,000 | — | MATCH | VN Annex VN.I "Fee for requesting substantive examination 720,000 VND" |
| OFF_ExaminationFee | each sheet > 6 | 32,000 | 32,000 | — | MATCH | VN Annex VN.I "each sheet in excess of six 32,000 VND" |
| Publication fee | always | — | 120,000 | — | MISSING | VN "Fee for publication 120,000 VND" |
| Publication fee | each drawing > 1 | — | 60,000 | — | MISSING | VN "each drawing in excess of one 60,000 VND" |
| Publication fee | each page > 6 | — | 10,000 | — | MISSING | VN "each sheet in excess of six 10,000 VND" |

Notes:
- The Summary gives "formality and substantive examination 900,000 VND" and "each sheet in excess of six 40,000 VND", the sums of the Annex items (180,000 + 720,000; 8,000 + 32,000), which the schedule models separately.
- The chapter's fee notes are stale: it still cites a 50 % filing-fee reduction "from 1 July 2021 until 30 June 2022" and annual fees "applicable from 1 July 2021 until 31 December 2021". VN-office reports that Circular 64/2025/TT-BTC sets the industrial-property charges (Section A of the Circular 263/2016 schedule: filing, grant, maintenance) at 50 % from 1 July 2025 to 31 December 2026. If confirmed from the official text, the filing fee in force today is 75,000, and the full amount returns on 1 January 2027 unless extended. The search, priority, examination and publication fees are Section B fees and are not reduced by that circular. A web search also reported, from a secondary source, that Circular 29/2026/TT-BTC waives the filing fee for filings made through VNeID; not saved, not applied.
- The eGuide states the search and substantive-examination fees as flat amounts. Not checked against Circular 263/2016 (not saved) whether they are charged per independent claim.

## WS: PCT-WS.ipf (WST)

Sources: eGuide WS, valid as from 1 January 2026 (sources/WS.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 500 | 500 | — | MATCH | WS "Filing fee 500 SAT"; Annex WS.I "Application fee 500 SAT" |
| OFF_SearchFee | optional | 30 | 30 | — | MATCH | WS Annex WS.I "Search fee 30 SAT" |

Notes: the chapter writes the Samoan tala as SAT; the schedule uses WST (same currency, as its own comment says). The 400 SAT maintenance fee falls due from the fifth year; it is not an entry fee.

## ZA: PCT-ZA.ipf (ZAR)

Sources: eGuide ZA, valid as from 1 September 2026 (sources/ZA.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-09-01 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 590 | 590 | — | MATCH | ZA "Filing fee 590 ZAR" |
| OFF_Renewal3 | ≥ 24 months from filing | 130 | 130 | — | MATCH | ZA "First annual fee ... due within three years from the international filing date. 130 ZAR" |

## ZM: PCT-ZM.ipf (USD; inherits bases/base_ap.ipf)

Sources: eGuide ZM, valid as from 28 May 2026 (sources/ZM.txt). Annex ZM.I: "Information not yet available".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-28 (chapter "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 115 | 115 | — | MATCH | ZM "National processing fee 115 USD" |
| OFF_Renewal | always | 50 | 50 | — | MATCH (condition doubtful, see notes) | ZM "First annual fee ... due within four years from the international filing date. 50 USD" |

Notes: the schedule charges the first annual fee unconditionally at entry. The chapter says it is due within four years of the international filing date, so it is normally not yet due at 30 months from priority. The baseline condition was left unchanged.

## ZW: PCT-ZW.ipf (ZWL; inherits bases/base_ap.ipf)

Sources: eGuide ZW, valid as from 1 January 2026 (sources/ZW.txt); law-firm note on the Patents (Amendment) Regulations of 18 April 2025 (sources/ZW-office.txt; secondary).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from" date).
Fees last changed: 18 April 2025 (amending regulations); foreign-applicant fees reported unchanged.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 6,000 ZWL | 6,000 ZWD (eGuide); USD amount not found | — | UNVERIFIED: the chapter's figure is in a defunct currency, and a secondary source says foreign applicants' fees are "stated in United States dollars" and payable in USD. The USD amount is not in any saved source | ZW "Filing fee 6,000 ZWD"; ZW-office "fees continue to be payable, as in previous regulations, in US dollars" |

Notes: currency question for the final report. The schedule keeps ZWL 6,000. The Zimbabwe dollar (ZWL) was itself replaced by the ZiG in 2024 (not in a saved source).

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| EP | 22 | 9 | 8 | 0 | 0 | 5 | 2026-04-01 (office table) | 1 Apr 2026 |
| US | 13 | 2 | 9 | 0 | 0 | 2 | 2025-01-19 (office table) | 19 Jan 2025 |
| UA | 5 | 4 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | not stated |
| UG | 6 | 5 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | not stated |
| UZ | 7 | 5 | 0 | 0 | 0 | 2 | 2026-01-01 (chapter) | not stated |
| VC | 2 | 1 | 0 | 0 | 0 | 1 | 2025-03-20 (chapter) | not stated |
| VN | 10 | 6 | 0 | 0 | 1 | 3 | 2026-05-28 (chapter) | not stated (secondary: 1 Jul 2025 reduction) |
| WS | 2 | 2 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| ZA | 2 | 2 | 0 | 0 | 0 | 0 | 2026-09-01 (chapter) | not stated |
| ZM | 2 | 2 | 0 | 0 | 0 | 0 | 2026-05-28 (chapter) | not stated |
| ZW | 1 | 0 | 0 | 0 | 1 | 0 | 2026-01-01 (chapter) | 18 Apr 2025 (secondary) |
| **Total** | **72** | **38** | **17** | **0** | **2** | **15** | | |
