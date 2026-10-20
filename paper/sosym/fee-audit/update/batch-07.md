# Batch 07: MN MW MX MZ NA NG NI NO NZ OA OM PA

Checked on 20 October 2026 against the saved eGuide chapters (`sources/XX.txt`) and, where one
could be reached, the office's own fee table (`sources/XX-office*.txt`). Baseline: the schedules as
they stood in `ipfees/src/IPFLang.Engine/Corpus` before this pass (all `VERSION '2024.1' EFFECTIVE
2024-01-01`). MN, MW, NA and NI, in which the batch could confirm no row, were first left
unchanged and were settled afterwards by the lead from the offices' own texts (see their records
and README.md).

## MN: PCT-MN.ipf (MNT)

Sources: eGuide MN, valid as from 15 May 2026 (sources/MN.txt); IPOM table of invention service
fees (sources/MN-office.txt, with the PDF), linked from the office's fee page (sources/MN-office-3.txt);
Government Resolution No. 151 of 19 May 2021, in force from 1 July 2021 (sources/MN-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2021-07-01 (the resolution's date of effect).
Fees last changed: 1 July 2021. Settled by the lead (the batch first left the row UNVERIFIED, the
office table not having been found).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 2,500 | 35,000 for up to five claims, 5,000 per further claim | 1 Jul 2021 | UPDATED (amount, basis): the office's table charges a new invention application by the number of claims; 2,500 MNT appears in no saved source. In force since 1 Jul 2021, so the baseline was already out of date in January 2024 | MN-office "1-5 томъёололтой 35000"; "-5-аас дээш томъёолол тус бүрд 35000 +Томъёоллын тоо *5000" |
| Substantive examination | on request | — | 40,000 for up to five claims, 5,000 per further claim | 1 Jul 2021 | MISSING | MN-office "Мөн чанарын шүүлт хийх 1-5 томъёололтой 40000" |

Notes: the eGuide chapter names no filing fee; the office's table, issued under Resolution 151,
does, and the chapter's grant and first annual fees (25,000 and 40,000 MNT) agree with the same
table. Foreign applicants pay "in freely convertible currency through an agent" (Annex B); the
schedule keeps MNT.

## MW: PCT-MW.ipf (MWK)

Sources: eGuide MW, valid as from 1 January 2026 (sources/MW.txt); Patents (Amendment)
Regulations, 2020, Government Notice No. 45, Malawi Gazette Supplement of 10 August 2020, as
reproduced on the Malawi Trade Portal (sources/MW-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2020-08-10 (the gazette date; the
Regulations state no other date of effect).
Fees last changed: 10 August 2020. Settled by the lead (the batch first left the row UNVERIFIED,
the amendment, which it cited as G.N. 43, not having been saved).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 500 | 20,000 (local); US$100 (foreign) | 10 Aug 2020 | UPDATED (amount): the 2020 Regulations replace the First Schedule; the chapter's 500 MWK is the earlier fee. In force since 2020, so the baseline was already out of date in January 2024 | MW-office "( b ) complete . . . . . ' 20,000.00 100.00"; MW.txt "Fee for lodging complete specification 500 MWK" |

Notes: currency question, as for GM. The schedule keeps MWK and carries the local fee; foreign
applicants are charged US$100 for the same item. The Regulations also charge "On application for
a patent (non-convention)" 50,000 / US$150 and a convention application 60,000 / US$180; the
chapter names only the specification fee for national phase entry, so neither is recorded.

## MX: PCT-MX.ipf (MXN, excluding 16 % IVA)

Sources: eGuide MX, valid as from 1 January 2026 (sources/MX.txt); IMPI Tarifa, last reform
published DOF 15-03-2024 (sources/MX-office.txt); ../AUDIT.md (19 October 2026), confirmed.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the chapter's "Valid as from"
date. The tariff gives only the publication date of its last reform, 15-03-2024, not an entry-into-force date).
Fees last changed: tariff last reformed DOF 15-03-2024 (which items changed then is not stated).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | OfficeElected = FALSE (Chapter I, Art. 1b) | 3,147 | 3,147 | by DOF 15-03-2024 | MATCH | MX-office "1b ... Capítulo I ... $3,147.00"; MX.txt "Article 22 3,147 MXN" |
| OFF_BasicNationalFee | OfficeElected = TRUE (Chapter II, Art. 1c) | 1,500 | 1,500 | by DOF 15-03-2024 | MATCH | MX-office "1c ... Capítulo II ... $1,500.00"; MX.txt "Article 39(1) 1,500 MXN" |
| OFF_PriorityFee | per priority (Art. 28) | 1,066.17 × n | 1,066.17 × n | by DOF 15-03-2024 | MATCH | MX-office "28 ... derecho de prioridad ... $1,066.17"; MX.txt "per priority 1,066.17 MXN" |
| OFF_SheetFee | each sheet over 30 (Arts. 1 b bis, 1 c bis) | 61 | 61 | by DOF 15-03-2024 | MATCH | MX-office "1 b bis ... por cada hoja adicional; $61.00"; MX.txt "Fee per sheet in excess of 30 61 MXN" |
| 50 % reduction | inventors (natural persons), micro/small industries, higher-education institutions, public research institutes; Arts. 1a–1f (not Art. 28) | — | 50 % | by DOF 15-03-2024 | MISSING | MX-office "CUARTA ... podrán pagar únicamente el 50% de las cuotas" |

Notes: agrees with AUDIT.md row for row. The chapter also lists the reduction (it names "small or
medium-sized enterprises"; the tariff says "micro o pequeñas industrias"). The schedule's
comment "#50% discount ... not clear from WIPO page" is now out of date but was left as it is.

## MZ: PCT-MZ.ipf (MZN)

Sources: eGuide MZ, valid as from 29 October 2024 (sources/MZ.txt); IPI fee table, Diploma
Ministerial 39/2017 of 15 May 2017, BR I Série No. 75, via WIPO Lex MZ026 (sources/MZ-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2017-07-14 (entry into force of the office
table: "entra em vigor sessenta dias a contar da data da sua publicação", publication 15 May 2017).
Fees last changed: 14 July 2017.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 8,100 | 8,100 | 14 Jul 2017 (before 1 Jan 2024) | MATCH | MZ-office "Transformação de pedido internacional em pedido nacional de patente 8,100.00"; MZ.txt "Filing fee 8,100 MZM" |
| OFF_Renewal (1st year) | RenMonth ≥ 12 | 1,275 | 1,275 | 14 Jul 2017 | MATCH | MZ-office "Anuidades 1." 1,275.00" (OCR); MZ.txt "for the 1st year 1,275 MZM" |
| OFF_Renewal (2nd year) | RenMonth ≥ 24 | 1,725 | 1,725 | 14 Jul 2017 | MATCH | MZ-office "2.' 1,725.00" (OCR); MZ.txt "for the 2nd year 1,725 MZM" |

Notes: the chapter labels the amounts "MZM" (the pre-2006 metical). The 2017 table is headed "Acto Taxa
ern Meticáis" (OCR of "em") and was adopted long after the 2006 redenomination, so the amounts are MZN and the
schedule's currency stands. The chapter says the 1st- and 2nd-year annual fees "must be paid when
entering the national phase". At entry (30/31 months) both of the schedule's month conditions always
hold, so they give the same result. Fee reductions exist only "if the Director of the Office so
orders", with no rate, so they are not recorded as MISSING. The table also has a
substantive-examination request fee (10,000.00 MT). The chapter does not list it among the
entry fees, so it is not recorded as MISSING.

## NA: PCT-NA.ipf (NAD)

Sources: eGuide NA, valid as from 14 March 2025 (sources/NA.txt); Industrial Property
Regulations 2018, GN 114, Government Gazette 6616 of 1 June 2018 (sources/NA-office.txt); BIPA's
own FAQ page (sources/NA-office-4.txt); Adams & Adams note of 19 Aug 2026 (sources/NA-office-2.txt);
Windhoek Express of 10 Aug 2026 (sources/NA-office-3.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2018-08-01 (the Regulations' date of effect).
Fees last changed: 1 August 2018 (Regulations); a consolidation from 1 July 2026 is reported by
secondary sources only. Settled by the lead (the batch first left the row UNVERIFIED).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 12 | 1,000 | 1 Aug 2018 | UPDATED (amount): the chapter's 12 NAD is the pre-2018 fee; the 2018 Regulations set N$1,000 and BIPA's own page states the same. In force since 2018, so the baseline was already out of date in January 2024 | NA-office "Application fee Patent 1000"; NA-office-4 "Patent application fees – N$ 1000" |

Notes: two secondary sources report that an amendment gazetted on 1 July 2026 merges the
application, registration and publication fees into one fee of N$1,500; Adams & Adams says "the
overall official fees remain unchanged", which fits 1,000 + 500 (registration and publication).
The amending gazette was not found, and BIPA's page still gives N$1,000, so the schedule carries
the application fee of the Regulations. The 2018 Regulations also charge "Claiming priority 200"
and "Multiple dependent claims 200" (Part B), and "Regional and international applications:
administration fee 200" (Part A); whether these survive the 2026 amendment is unknown, so they are
not recorded as MISSING.

## NG: PCT-NG.ipf (NGN)

Sources: eGuide NG, valid as from 15 September 2023, last updated 28 June 2024 (sources/NG.txt).
No official office fee table found (search returned only agents' guides).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2023-09-15 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 25,000 | 25,000 | not stated | MATCH | NG.txt "Filing fee 25,000 NGN" |

Notes: the chapter is older than the baseline but agrees with it. It is the only source, so the
match rests on a 2023 chapter. Reductions: "None".

## NI: PCT-NI.ipf (NIO)

Sources: eGuide NI, valid as from 1 January 2026 (sources/NI.txt); Ley No. 1025 reforming Art. 128
of Ley No. 354, La Gaceta Diario Oficial of 3 April 2020 (No. 65), via FAOLEX (sources/NI-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2020-04-03 (lead decision, README.md).
Fees last changed: 3 April 2020 (Ley 1025 "entrará en vigencia a partir de su publicación en La Gaceta").

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 200 (NIO) | CA$240 (first 20 claims), payable in NIO at the Central Bank rate | 3 Apr 2020 (before 1 Jan 2024) | UPDATED (amount, currency; lead decision, README.md): the law sets the fee in Central American pesos, at par with the US dollar. The chapter's "200 NIO" predates Ley 1025 | NI-office "Solicitud de patente de invención: Por las primeras 20 reivindicaciones $CA240.00"; NI.txt "Filing fee 200 NIO" |
| claim fee | each claim over 20 | — | CA$20 | 3 Apr 2020 | MISSING | NI-office "Por cada reivindicación adicional a las 20 $CA20.00" |
| 50 % reduction | natural persons with prior-year income under CA$6,000; micro/small enterprises; public higher-education and research institutes | — | 50 % | 3 Apr 2020 | MISSING | NI-office "se reducirán al 50% si el solicitante es una persona natural" |

Notes: the chapter instead says "The national fee is reduced by 75% where the application is filed
by a natural person", which also predates Ley 1025. The law says the CA$ amount "se cancelará en
moneda nacional, aplicando la tasa de cambio que el Banco Central de Nicaragua fijare". The
schedule could model the fee as `240<USD>` (CA$ is at par with USD) or keep NIO. This is a decision
for the lead.

## NO: PCT-NO.ipf (NOK)

Sources: eGuide NO, valid as from 1 August 2026 (sources/NO.txt); Patentstyret "NIPO's fees",
last modified 25 September 2026 (sources/NO-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-08-01 (the chapter's "Valid as from" date.
The office page gives no effective date for the national fees; its only effective date, 1 August 2026, is for the PCT receiving-office fees).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 4,650 | 6,050 | not stated | UPDATED (amount) | NO-office "Application to extend a PCT application to Norway ... more than 20 full-time employees ... NOK 6 050"; NO.txt "Basic fee, including examination fee ... 6,050 NOK" |
| OFF_ClaimFee | each claim over 10 | 250 | 330 | not stated | UPDATED (amount) | NO-office "Additional fee for each new patent claim over ten \| NOK 330"; NO.txt "Claim fee for each claim in excess of 10 ... 330 NOK" |
| OFF_Renewal (1st year) | always | 700 | 910 | not stated | UPDATED (amount) | NO-office "1st fee year \| NOK 910"; NO.txt "Annual fees for the first three years ... per year 910 NOK" |
| OFF_Renewal (2nd year) | RenMonth ≥ 12 | 700 | 910 | not stated | UPDATED (amount) | NO-office "2nd fee year \| NOK 910" |
| OFF_Renewal (3rd year) | RenMonth ≥ 24 | 700 | 910 | not stated | UPDATED (amount) | NO-office "3rd fee year \| NOK 910" |
| reduced basic fee | natural person or entity with ≤ 20 employees | — | 1,110 (office) / 1,100 (chapter) | not stated | MISSING | NO-office "20 or less full-time employees ... NOK 1 110"; NO.txt "6,050 NOK (1,100 NOK)" |

Notes: on the reduced rate the office's PCT-entry row (1 110) differs from its own national-filing
row and from the chapter (both 1 100). The baseline renewal amount, 700, is now the late-payment
supplement ("Supplement for late payment 700 NOK"). The chapter says the first three annual fees fall due at
24 months from the international filing date, and the schedule's three yields give all three at
entry. That is consistent.

## NZ: PCT-NZ.ipf (NZD, excluding GST)

Sources: eGuide NZ, valid as from 1 January 2026 (sources/NZ.txt); IPONZ "Patent fees"
(sources/NZ-office.txt), "IPONZ fees were updated on 13 February 2020".
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2020-02-13 (the effective date of the office's current table).
Fees last changed: 13 February 2020.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 250 | 250 | 13 Feb 2020 | MATCH | NZ-office "PCT - Entry into national phase \| $250"; NZ.txt "Filing fee ... 250 NZD" |

Notes: the examination request (750 NZD) and the excess-claims fee (120 NZD per 5 claims over 25) are
not due at entry, so they are not MISSING. The schedule's comments on maintenance and examination
still hold.

## OA: PCT-OA.ipf (XAF; regional phase, inherits bases/base_oa.ipf)

Sources: eGuide OA, valid as from 3 July 2026 (sources/OA.txt), "National fees (with effect since
1 January 2025)"; OAPI "Taxes applicable to patents and certificates of addition"
(sources/OA-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-01-01 (the date the chapter gives for the current national fees).
Fees last changed: 1 January 2025.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 225,000 | 225,000 | — | MATCH | OA-office "Filing (paper) of patent covering the first year \| 225 000"; OA.txt "Filing fee ... 225,000 XAF" |
| OFF_PublicationFee | always | 365,000 | 365,000 | — | MATCH | OA-office "Publication of a patent application ... \| 365 000"; OA.txt "Publication fee ... 365,000 XAF" |
| OFF_PriorityFee | per priority | 63,000 × n | 75,000 × n | 1 Jan 2025 | UPDATED (amount) | OA-office "Claim of priority, by priority \| 75 000"; OA.txt "Fee for priority claims, per priority 75,000 XAF" |
| OFF_ClaimFee | each claim over 10 | 45,000 | 65,000 | 1 Jan 2025 | UPDATED (amount) | OA-office "Supplement per claim beyond the 10th \| 65 000"; OA.txt "Claim fee for each claim in excess of 10 65,000 XAF" |
| Off_AcceptanceFee | 11–20 sheets | 120,000 | 120,000 | — | MATCH | OA-office "11 to 20 typed pages ... \| 120 000"; OA.txt "for 11 to 20 sheets 120,000 XAF" |
| Off_AcceptanceFee | 21–30 sheets | 300,000 | 300,000 | — | MATCH | OA-office "21 to 30 pages or plates \| 300 000" |
| Off_AcceptanceFee | 31–40 sheets | 600,000 | 600,000 | — | MATCH | OA-office "From 31 to 40 pages or plates ... \| 600 000" |
| Off_AcceptanceFee | > 40 sheets, per further set of 10 | 600,000 + 80,000 × ⌈(n−40)/10⌉ | same | — | MATCH | OA-office "at a rate of 80,000 FCFA per undivided increment of 10 pages"; OA.txt "after 40 for each set of 10 sheets 80,000 XAF" |
| OFF_Renewal (2nd year) | RenMonth ≥ 12 | 220,000 | 270,000 | 1 Jan 2025 | UPDATED (amount) | OA-office "From year 2 to year 5, by year \| 270 000"; OA.txt "for the 2nd to the 3rd year, per year 270,000 XAF" |
| OFF_Renewal (3rd year) | RenMonth ≥ 24 | 220,000 | 270,000 | 1 Jan 2025 | UPDATED (amount) | same |
| examination fee | always | — | 250,000 | 1 Jan 2025 | MISSING | OA-office "Examination of the application \| 250 000"; OA.txt "Examination fee 250,000 XAF" |

Notes: the OAPI table and the chapter agree on every figure. The schedule's comment "need to verify
with office if really payable upon entry" on OFF_Renewal is answered by the chapter, which lists the
2nd- and 3rd-year annual fees among the entry fees. The OAPI page labels the filing fee "Filing
(paper)"; no separate electronic rate is published.

## OM: PCT-OM.ipf (OMR)

Sources: eGuide OM, valid as from 1 January 2026 (sources/OM.txt); Oman Observer, 31 Oct 2020
(sources/OM-office.txt; a press report, not an office table). No office fee table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 300 | 300 | not stated | MATCH | OM.txt "Filing fee: 300 OMR (200 OMR)" |
| reduced filing fee | filing by an individual | — | 200 | not stated | MISSING | OM.txt "The amount in parentheses is applicable in case of filing by an individual" |
| 90 % / 50 % reductions | researchers and students (90 %); small enterprises (50 %); first five years | — | 90 % / 50 % | Res. 206/2018 | MISSING | OM.txt "reduced by 90% ... and by 50% where the application is filed by a small enterprise" |

Notes: the 2020 press report says applicants pay "application fees RO 200 for individuals or
companies" after a ministry reduction. The more recent chapter (2026) keeps 300 OMR with 200 OMR
for individuals, so the baseline stands. This should be confirmed with the Ministry of Commerce,
Industry and Investment Promotion if an office table can be found. The chapter's Annex
also lists a "Request for examination fee 300 OMR" without saying it is due at entry, so it is not
recorded as MISSING.

## PA: PCT-PA.ipf (USD)

Sources: eGuide PA, valid as from 1 January 2026 (sources/PA.txt). No office fee table found
(DIGERPI tariff not located by search).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 128.50 | 128.50 | not stated | MATCH | PA.txt "Filing fee 128.50 USD" |
| reduced filing fee | applicant who is the inventor and cannot pay | — | 10 % of the filing fee | not stated | MISSING | PA.txt "he may be entitled to only pay 10% of the filing fee" |

## Batch summary

Rows counts every table row, MISSING rows included.

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| MN | 2 | 0 | 1 | 0 | 0 | 1 | 2021-07-01 (Resolution 151) | 1 Jul 2021 |
| MW | 1 | 0 | 1 | 0 | 0 | 0 | 2020-08-10 (gazette) | 10 Aug 2020 |
| MX | 5 | 4 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | DOF 15-03-2024 (last tariff reform) |
| MZ | 3 | 3 | 0 | 0 | 0 | 0 | 2017-07-14 (office table) | 14 Jul 2017 |
| NA | 1 | 0 | 1 | 0 | 0 | 0 | 2018-08-01 (Regulations) | 1 Aug 2018 (consolidation of 1 Jul 2026 reported) |
| NG | 1 | 1 | 0 | 0 | 0 | 0 | 2023-09-15 (chapter) | not stated |
| NI | 3 | 0 | 1 | 0 | 0 | 2 | 2020-04-03 (Ley 1025) | 3 Apr 2020 (Ley 1025) |
| NO | 6 | 0 | 5 | 0 | 0 | 1 | 2026-08-01 (chapter) | not stated |
| NZ | 1 | 1 | 0 | 0 | 0 | 0 | 2020-02-13 (office table) | 13 Feb 2020 |
| OA | 11 | 6 | 4 | 0 | 0 | 1 | 2025-01-01 (chapter's stated effect date) | 1 Jan 2025 |
| OM | 3 | 1 | 0 | 0 | 0 | 2 | 2026-01-01 (chapter) | not stated |
| PA | 2 | 1 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | not stated |
| **Total** | **39** | **17** | **13** | **0** | **0** | **9** | | |
