# Batch 05: IQ IR IS IT JM JO JP KE KG KH KN KP

Checked on 20 October 2026 against the fees in force that day. Baseline: IPFees `beab6fd`
(every schedule carried `VERSION '2024.1' EFFECTIVE 2024-01-01`). Every eGuide chapter was
retrieved on 20 October 2026 (`sources/XX.txt`). The only office table that could be reached and
saved is Iceland's (`sources/IS-office.txt`). The JPO fee pages named in `../AUDIT.md` now return
"Page Not Found" at both the English and the Japanese URL, so JP rests on the saved chapter and on
the AUDIT record. patent.kg failed its TLS check (certificate name mismatch), so KG rests on the
chapter alone. For the other offices only the chapter was used.

## IQ: PCT-IQ.ipf (IQD)

Sources: eGuide IQ, valid as from 1 January 2026 (sources/IQ.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated. The chapter's 1 January 2026 changes touch only the USD equivalent ("Filing fee: 55 USD").

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 77.075 | 77,075 | not stated | UPDATED (amount) | IQ "Filing fee: 77,075 IQD 55 USD" |

Notes: the baseline value `77.075` comes from reading the thousands separator in "77,075 IQD"
as a decimal point. As a result the baseline charged about 1/1000 of the fee. The schedule now
carries 77075 IQD. The chapter has no Annex IQ.I ("Information not yet available").

## IR: PCT-IR.ipf (IRR)

Sources: eGuide IR, valid as from 1 January 2026 (last updated 27 March 2026) (sources/IR.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | other than natural person / natural person | 100,000/10,000 | 2,500,000/500,000 | not stated | UPDATED (amount) | IR "Filing fee 2,500,000 IRR — in case of filing by a natural person 500,000 IRR" |

Notes: the annual fees (5,000,000/1,000,000 IRR per year, payable from two years after the
international filing date) and the grant fee are not entry fees and are not modelled; this is
unchanged.

## IS: PCT-IS.ipf (ISK)

Sources: eGuide IS, valid as from 1 January 2026 (sources/IS.txt); Regulation No. 1279/2024 on fees for patents, trademarks and designs, current version, which enters into force on 15 February 2025 (sources/IS-office.txt, .pdf).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-02-15 (entry into force of Regulation 1279/2024, "öðlast gildi 15. febrúar 2025").
Fees last changed: 15 February 2025 (Regulation 1279/2024 repealed Regulation 536/2023).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 67,800 | 76,000 | 15 Feb 2025 | UPDATED (amount) | IS-office "a. umsóknargjald 76.000"; IS "Application fee ... 76,000 ISK" |
| OFF_ClaimFee | per claim over 10 | 4,400 | 4,900 | 15 Feb 2025 | UPDATED (amount) | IS-office "viðbótargjald fyrir hverja einkaleyfiskröfu umfram tíu 4.900"; IS "Claim fee for each claim in excess of 10 ... 4,900 ISK" |
| OFF_Renewal3 | annual fees, years 1–3, age ≥ 24 months | 34,800 (11,600 × 3) | 39,000 (13,000 × 3) | 15 Feb 2025 | UPDATED (amount) | IS-office "Gjaldár 1. 13.000 / 2. 13.000 / 3. 13.000"; IS Annex "for the 1st to the 3rd year per year 13,000 ISK" |

Notes: the chapter contradicts itself on the first three annual fees. Its summary gives "Annual
fees for the first three years ... 36,000 ISK", while its Annex IS.I gives 13,000 ISK per year. The
regulation agrees with the annex (13,000 × 3 = 39,000), and that amount is used. For the 4th year
onwards, the chapter's annex is older than the regulation (for example 4th year 15,000 in the
chapter against 24,000 in the regulation), but those years are not modelled. The additional fee
for late furnishing of the translation (20,500 ISK) is a late-act surcharge and is not modelled.
The regulation's header reads "Útgáfa í gildi frá 15. nóv. 2024", but Art. 22 sets entry into
force at 15 February 2025, and that date was used.

## IT: PCT-IT.ipf (EUR)

Sources: eGuide IT, valid as from 1 January 2026 (sources/IT.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated. The chapter cites the Ministerial Decree of 13 November 2019.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (online) | 50 | 50 | not stated | MATCH | IT "application with up to 10 pages — online 50 EUR" |
| OFF_ClaimFee | per claim over 10 (online) | 45 | 45 | not stated | MATCH | IT "for each claim exceeding 10 — online 45 EUR" |
| OFF_SheetFee | per sheet over 10 (50 EUR/sheet) | 50/50/50 per sheet in bands 11–20, 21–50, >50 | none online | — | REMOVED | IT "application with 11 to 20 pages — online 50 EUR ... 21 to 50 pages — online 50 EUR ... more than 50 pages — online 50 EUR" |
| Exemption for universities, public research institutions, defence and agricultural/food/forestry administrations | applicant category | — | no filing fee | — | MISSING | IT "No filing fee is payable if the application is filed by universities, public research institutions..." |

Notes: the filing fee depends on the page count only for paper filing (120/160/400/600 EUR by
band). Even on paper, these are total filing fees per band, not per-sheet rates. Filed online, as
the corpus assumes, the fee is a flat 50 EUR in every band. The baseline added 50 EUR for each
sheet over 10 on top of that. `OFF_SheetFee` and its two `VERIFY` directives were removed, and a
one-line comment says why. The `SheetCount` input stays defined (DEFINEs are not touched). The
summary bases the claim count on the claims "as published", while Annex IT.I says "as amended".
The schedule takes the claim count as given either way.

## JM: PCT-JM.ipf (JMD)

Sources: eGuide JM, valid as from 1 January 2026 (sources/JM.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 30,000 | 30,000 | not stated | MATCH | JM "Patents Filing fee 30,000 JMD" |
| OFF_AgentAppointmentFee | always | 4,500 | 4,500 | not stated | MATCH | JM "Appointment or change of an agent 4,500 JMD" |
| OFF_SheetFee (publication of translation) | up to 4 pages | 3,500 | 3,500 | not stated | MATCH | JM "Publication of translation: — up to 4 pages 3,500 JMD" |
| OFF_SheetFee (publication of translation) | per page over 4 | 500 | 500 | not stated | MATCH | JM "for each page in addition to 4 pages 500 JMD" |

## JO: PCT-JO.ipf (JOD)

Sources: eGuide JO, valid as from 28 May 2026 (sources/JO.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-28 (chapter's "Valid as from" date).
Fees last changed: not stated. The 28 May 2026 changes are to payment and procedure text, not to the amounts.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | company or organisation / individual | 200/100 | 200/100 | not stated | MATCH | JO "Filing fee filing by a company or an organization 200 JOD filing by an individual 100 JOD" |

Notes: the examination fee (200/100 JOD) is payable upon invitation, and the grant and annual fees
are not entry fees. None of them is modelled, as before.

## JP: PCT-JP.ipf (JPY)

Sources: eGuide JP, valid as from 1 July 2026 (last updated 22 September 2026) (sources/JP.txt); JPO fee schedule as recorded in ../AUDIT.md (English page headed "on or after April 1, 2022"; Japanese page updated 6 April 2026). Both JPO URLs returned "Page Not Found" on 20 October 2026, so neither could be saved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2022-04-01 (effective date of the JPO's current fee table, as recorded in AUDIT.md).
Fees last changed: 1 April 2022 (JPO schedule, per AUDIT.md). The examination fees modelled are those for international applications filed on or after 1 April 2019.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 14,000 | 14,000 | before 1 Jan 2024 | MATCH | JP "National fee 14,000 JPY"; AUDIT JP row 1 |
| OFF_ExaminationFee | no ISR | 138,000 + 4,000/claim | 138,000 + 4,000/claim | IA filed ≥ 1 Apr 2019 | MATCH | JP "(a) where no international search report has been established: 138,000 JPY plus, for each claim 4,000 JPY" |
| OFF_ExaminationFee | ISR by JPO | 83,000 + 2,400/claim | 83,000 + 2,400/claim | IA filed ≥ 1 Apr 2019 | MATCH | JP "(b) ... by the Office: 83,000 JPY plus, per claim 2,400 JPY" |
| OFF_ExaminationFee | ISR by another ISA | 124,000 + 3,600/claim | 124,000 + 3,600/claim | IA filed ≥ 1 Apr 2019 | MATCH | JP "(c) ... other than the Office: 124,000 JPY plus, per claim 3,600 JPY" |
| OFF_ExaminationFee (ExFee4, declared but never yielded) | report by a searching organisation designated under Japanese law | 111,000 + 3,200/claim | 110,000 + 3,200/claim | IA filed ≥ 1 Apr 2019 | UPDATED (amount) | JP "(d) ... designated under Japanese law: 110,000 JPY plus, per claim 3,200 JPY"; AUDIT JP row 5 |
| Reductions for individuals, SMEs, micro enterprises, academic institutions | applicant category | — | exist (amounts in JPO leaflet) | — | MISSING | JP "reductions are available to individuals, small and medium-sized enterprises, micro enterprises, academic institutions" |

Notes: the AUDIT findings are confirmed against the saved chapter. Only `LET ExFee4` changed,
and that constant is still not yielded. The schedule does not condition the examination fee on
the international filing date. The pre-1 April 2019 rates (118,000/71,000/106,000/94,000) are not
modelled; this is unchanged.

## KE: PCT-KE.ipf (USD)

Sources: eGuide KE, valid as from 1 January 2026 (sources/KE.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 150 | 150 | not stated | MATCH | KE "National processing fee: 3,000 KES 150 USD" |
| OFF_ExaminationFee | optional | 250 | 250 | not stated | MATCH | KE "Request for substantive examination 5,000 KES 250 USD" |
| OFF_Renewal | per year due, age ≥ 12 and ≥ 24 months | 300 | 300 | not stated | MATCH | KE "Annual fee for the 2nd year: ... 2,000 KES 300 USD"; "from the 2nd year to the 7th year, per year 2,000 KES 300 USD" |

Notes: under the chapter, applicants who neither reside nor have a principal place of business in
Kenya pay in USD, which matches the schedule's currency.

## KG: PCT-KG.ipf (KGS)

Sources: eGuide KG, valid as from 28 May 2026 (sources/KG.txt). The office site (patent.kg) failed its TLS check.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-28 (chapter's "Valid as from" date).
Fees last changed: not stated. The 28 May 2026 changes are to payment details and the reinstatement fee, not to the modelled amounts.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 8,000 | 4,000 or 8,000 | — | UNVERIFIED | the chapter contradicts itself: summary "Filing fee ... 4,000 KGS" against Annex KG.I "Filing and provisional examination fee 8,000 KGS"; office site unreachable |
| OFF_IndependentClaimFee | per independent claim over 1 | 1,500 | 1,500 | not stated | MATCH | KG "Claim fee for each independent claim in excess of one ... 1,500 KGS" |
| OFF_ExaminationFee | optional, base + per independent claim over 1 | 15,000 + 7,500 | 15,000 + 7,500 | not stated | MATCH | KG "Examination fee ... 15,000 KGS"; "Claim examination fee for each independent claim in excess of one ... 7,500 KGS" |
| OFF_ExaminationFee reduction | ISR established, no IPRP | −25 % | −25 % | not stated | MATCH | KG "reduced by 25% where an international search report has been established" |
| OFF_ExaminationFee reduction | IPRP established | −50 % | −50 % | not stated | MATCH | KG "by 50% where an international preliminary examination report has been established" |
| 90 % reduction for natural persons and non-commercial organisations | applicant category | — | −90 % | — | MISSING | KG "Fees are reduced by 90% where the applicant is a natural person or a non-commercial organization" |

Notes: the summary lists a "Renewal fee for the 3rd year 8,500 KGS" among the national fees, but
KG.10 says the first renewal fee is paid together with the grant fee. Since it is not an entry
fee, it is not recorded as MISSING. The schedule applies the ISR/IPRP reduction to the base
examination fee only. The chapter's sentence sits under the per-claim examination fee and says
"the examination fee is reduced"; the condition is left as it was. Baseline oddity, left
unchanged: `VERIFY MONOTONIC FEE OFF_IndependentClaimFee WITH RESPECT TO ClaimCount` names
`ClaimCount`, but the fee depends on `IndependentClaimCount`.

## KH: PCT-KH.ipf (KHR)

Sources: eGuide KH, valid as from 1 January 2026 (sources/KH.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 320,000 | 320,000 | not stated | MATCH | KH "Filing fee ... 320,000 KHR" |
| OFF_ClaimFee | per claim over 10 | 20,000 | 20,000 | not stated | MATCH | KH "Claim fee for each claim in excess of 10 20,000 KHR" |
| OFF_Renewal | 2nd year, age ≥ 12 months | 80,000 | 80,000 | not stated | MATCH | KH "for the 2nd year 80,000 KHR" |
| OFF_Renewal | 3rd year, age ≥ 24 months | 12,000 | 12,000 | not stated | MATCH | KH "for the 3rd year 12,000 KHR" |

Notes: the 3rd-year annual fee of 12,000 KHR sits oddly between 80,000 (2nd year) and 160,000
(4th year) and may be a typo for 120,000 in the chapter. It is kept because that is what the
source says.

## KN: PCT-KN.ipf (XCD)

Sources: eGuide KN, valid as from 13 March 2025 (sources/KN.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 400 | 400 | not stated | MATCH | KN "Patents Filing fee 400 XCD" |
| OFF_PublicationFee | always | 50 | 50 | not stated | MATCH | KN "Fee for publication 50 XCD" |
| OFF_Renewal | 1st–3rd year (always, ≥ 12, ≥ 24 months) | 100 | 100 | not stated | MATCH | KN "for the 1st to the 3rd year, per year 100 XCD" |

Notes: the chapter says annual fees are due "each year starting one year after the filing date".
The schedule always yields one annual fee, even before 12 months; the condition is left as it was.

## KP: PCT-KP.ipf (EUR)

Sources: eGuide KP, valid as from 13 March 2025 (sources/KP.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as from" date).
Fees last changed: not stated. The national fee was "Published in PCT Gazette No. 28/2003, of 10 July 2003".

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 230 | 230 | 10 Jul 2003 (publication) | MATCH | KP "Filing fee Equivalent in KPW of 230 EUR" |

Notes: the fee is payable as the equivalent in KPW of an amount fixed in EUR. The schedule keeps
EUR. The additional fee for late furnishing of the translation (30 EUR per month) is a late-act
surcharge and is not modelled.

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| IQ | 1 | 0 | 1 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| IR | 1 | 0 | 1 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| IS | 3 | 0 | 3 | 0 | 0 | 0 | 2025-02-15 (Regulation 1279/2024) | 15 Feb 2025 |
| IT | 4 | 2 | 0 | 1 | 0 | 1 | 2026-01-01 (chapter) | not stated (MD 13 Nov 2019) |
| JM | 4 | 4 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| JO | 1 | 1 | 0 | 0 | 0 | 0 | 2026-05-28 (chapter) | not stated |
| JP | 6 | 4 | 1 | 0 | 0 | 1 | 2022-04-01 (JPO table, per AUDIT.md) | 1 Apr 2022 |
| KE | 3 | 3 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| KG | 6 | 4 | 0 | 0 | 1 | 1 | 2026-05-28 (chapter) | not stated |
| KH | 4 | 4 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| KN | 3 | 3 | 0 | 0 | 0 | 0 | 2025-03-13 (chapter) | not stated |
| KP | 1 | 1 | 0 | 0 | 0 | 0 | 2025-03-13 (chapter) | not stated (published 10 Jul 2003) |
| Total | 37 | 26 | 6 | 1 | 1 | 3 | | |
