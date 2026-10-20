# Batch 03: CZ DE DJ DK DO DZ EA EC EE EG ES

Checked on 20 October 2026 against the WIPO PCT Applicant's Guide chapters saved in
`sources/XX.txt` and, where the office's site answered, the office's own fee table
(`sources/XX-office*.txt`). Schedules: `ipfees/src/IPFLang.Engine/Corpus/PCT-XX.ipf`, baseline
IPFees `beab6fd`. One row per `YIELD` condition of a modelled fee; MISSING rows count in the
summary.

## CZ: PCT-CZ.ipf (CZK)

Sources: eGuide CZ, valid as from July 1, 2026 (sources/CZ.txt); UPV fee overview "Zákon č. 634/2004 Sb. ... Sazebník správních poplatků", file dated 08/2025 (sources/CZ-office.txt); UPV fee page (sources/CZ-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-01 (eGuide "Valid as from"; the UPV overview states no effective date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 1,200 | 1,200 | not stated | MATCH | CZ-office "a) Přijetí přihlášky vynálezu ... Kč 1 200"; CZ "Filing fee 1,200 CZK" |
| OFF_ExaminationFee | basic | 3,000 | 3,000 | not stated | MATCH | CZ-office "úplného průzkumu přihlášky vynálezu ... Kč 3 000"; CZ "— basic fee 3,000 CZK" |
| OFF_ExaminationFee | per claim > 10 | 500 | 500 | not stated | MATCH | CZ-office "11. a každý další uplatněný patentový nárok Kč 500" |
| (filing fee reduction) | applicants are solely the inventors | – | 600 (50%) | not stated | MISSING | CZ-office "výlučně původce(i) Kč 600"; CZ "reduced by 50% where the applicant is also the inventor" |

Notes: none.

## DE: PCT-DE.ipf (EUR)

Sources: eGuide DE, valid as from April 10, 2026 (sources/DE.txt); DPMA "Patent fees" page (sources/DE-office.txt); DPMA leaflet A 9510.1 "Information Concerning Costs, Fees and Expenses", Edition July 2026 (sources/DE-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-10 (eGuide "Valid as from"; the DPMA leaflet gives an edition date, not an effective date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (up to 10 claims) | 60 | 60 | not stated | MATCH | DE-office-2 "311 150 - containing up to ten patent claims ... 60" |
| OFF_ClaimFee | per claim > 10 | 30 | 30 | not stated | MATCH | DE-office-2 "311 160 ... fee 311 150 plus ... 30 for each additional claim" |
| OFF_ExaminationFee | ISR available | 150 | 150 | not stated | MATCH | DE-office-2 "311 300 ... 150"; DE "(i) where an international search report has been established 150 EUR" |
| OFF_ExaminationFee | no ISR | 350 | 350 | not stated | MATCH | DE-office-2 "311 400 ... 350" |
| OFF_Renewal3 | RenMonth ≥ 18 | 70 | 70 | not stated | UPDATED (condition: due at 24 months; lead decision, README.md) | DE-office-2 "312 030 for the 3rd year of the patent ... 70" |
| (filing fee exemption) | DPMA was the receiving Office | – | 0 | not stated | MISSING | DE "No filing fee is payable if the international application was filed with the German Patent and Trade Mark Office as receiving Office" |

Notes: the 40/20 EUR electronic rates on the DPMA page are for national applications (311 000/311 050);
the PCT national fee (311 150/311 160) has no electronic rate. Condition doubt, left unchanged: the
3rd-year fee is due at the end of the month containing the second anniversary (24 months) of the
international filing date (DE.07, DE "Annual fee for the 3rd year ... second anniversary (24 months)"),
but the schedule charges it from 18 months.

## DJ: PCT-DJ.ipf (DJF)

Sources: eGuide DJ, valid as from January 1, 2026 (sources/DJ.txt); ODPIC "Tableau des redevances des brevets" (sources/DJ-office.txt, undated).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (eGuide "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 172,500 | 172,500 | not stated | MATCH | DJ-office "Droit de dépôt d'un brevet d'invention \| 172 500 FDJ"; DJ "Filing fee 172,500 DJF" |

Notes: payment is accepted in DJF "or the equivalent amount in US dollars" (DJ); the schedule keeps DJF.
The eGuide says the filing fee includes maintenance for the first 5 years; the ODPIC table lists a
"1ère periode | 50 000 FDJ" maintenance fee that the eGuide does not. Neither is modelled; noted only.

## DK: PCT-DK.ipf (DKK)

Sources: eGuide DK, valid as from March 26, 2026 (sources/DK.txt); DKPTO "Prices and Fees" page (sources/DK-office.txt); DKPTO "Fees of the Danish Patent and Trademark Office per 1 January 2026" (sources/DK-office-2.txt); DKPTO news of 19-12-2025 (sources/DK-office-3.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (effective date of the DKPTO table).
Fees last changed: 1 January 2026 (renewal fees: for fees due after 31 March 2026).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 3,000 | 3,600 | 1 Jan 2026 | UPDATED (amount) | DK-office-2 "a. Basic fee 3,000 3,600" (2025 / 2026 columns); DK "Basic fee 3,600 DKK" |
| OFF_ClaimFee | per claim > 10 | 300 | 360 | 1 Jan 2026 | UPDATED (amount) | DK-office-2 "Additional fee for each patent claim in excess of 300 360 10" |
| OFF_Renewal | 1st year (always) | 500 | 660 | 1 Apr 2026 | UPDATED (amount, condition: due at 24 months; lead decision, README.md) | DK-office-2 "1st fee year 550 660"; DK "Annual fees for the first three years ... 660 DKK" |
| OFF_Renewal | 2nd year (RenMonth ≥ 12) | 500 | 660 | 1 Apr 2026 | UPDATED (amount, condition: due at 24 months; lead decision, README.md) | DK-office-2 "2nd fee year 550 660" |
| OFF_Renewal | 3rd year (RenMonth ≥ 24) | 500 | 660 | 1 Apr 2026 | UPDATED (amount) | DK-office-2 "3rd fee year 550 660" |

Notes: renewal rates: "The price adjustment of the renewal fees is valid for fees due after the 31st of
March 2026" (DK-office-2). The 2025 column already shows 550 per year, so the baseline 500 was out of
date by 2025 at the latest (the date of the 500 → 550 step is not in the saved sources). The basic and
claim fees at baseline equal the 2025 column. The eGuide Annex DK.I still prints some 2025 figures
(publication fee 2,230, late translation 1,100), the office table is newer. Condition doubt, left
unchanged: the three first annual fees are all due at 24 months from the international filing date
(DK "due on the last day of the month containing the second anniversary (24 months)"); the schedule
charges year 1 always and year 2 from 12 months.

## DO: PCT-DO.ipf (DOP)

Sources: eGuide DO, valid as from February 5, 2026 (sources/DO.txt); ONAPI "Tasas de servicio vigentes a partir del 31 de marzo del año 2023" (sources/DO-office-2.txt; the page linking it is sources/DO-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2023-03-31 (effective date of the ONAPI table).
Fees last changed: 31 March 2023.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (up to 30 sheets) | 13,225 | 13,225 | 31 Mar 2023 | MATCH | DO-office-2 "Registro de Patente de Invención RD$13,225.00" |
| OFF_SheetFee | per sheet > 30 | 75 | 75 | 31 Mar 2023 | MATCH | DO-office-2 "75.00 por cada hoja adicional ... superan las 30 hojas" |
| OFF_PriorityFee | per priority | 2,645 | 2,645 | 31 Mar 2023 | MATCH | DO-office-2 "Reclamo de Prioridad de Patente de Invención por cada una ... RD2,645.00" |
| OFF_Renewal | 2nd year (RenMonth ≥ 12) | 6,610 | 6,610 | 31 Mar 2023 | MATCH | DO-office-2 "Mantenimiento de Vigencia de Patentes de Invenciones RD$ 6,610.00 de 2 a 4 años" |
| OFF_Renewal | 3rd year (RenMonth ≥ 24) | 6,610 | 6,610 | 31 Mar 2023 | MATCH | same |
| (reduction) | applicant is the inventor, economic hardship | – | up to 90% | not stated | MISSING | DO "The fees are reduced by up to 90% where the applicant is the inventor ..." |

Notes: the examination fee (15,870) and publication fee (3,965) in Annex DO.I are not listed among the
national fees due at entry and are not modelled.

## DZ: PCT-DZ.ipf (DZD)

Sources: eGuide DZ, valid as from January 1, 2026, last updated June 24, 2026 (sources/DZ.txt); INAPI "Taxes relatives aux brevets d'invention et aux certificats d'addition" (sources/DZ-office.txt, undated).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (the eGuide lists the new applicant-category tariffs under "Change(s) as of January 1, 2026"; INAPI states no date).
Fees last changed: 1 January 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 7,500 | 15,000 | 1 Jan 2026 | UPDATED (amount) | DZ-office "762-01 Taxe de dépôt et de première annuité Entreprise ... 15000"; DZ "Filing fee and maintenance fee for the 1st year: — Company 15,000 DZD" |
| OFF_PriorityFee | per priority | 2,000 | 10,000 | 1 Jan 2026 | UPDATED (amount) | DZ-office "762-03 Taxe de revendication de priorité Entreprise ... 10000"; DZ "— Company 10,000 DZD" |
| OFF_SheetFee | per 5 pages > 10 | 1,200 | 5,000 | 1 Jan 2026 | UPDATED (amount) | DZ-office "762-21 ... chaque 5 pages supplémentaires au-delà des 10 premières : Entreprises ... 5000"; DZ "Surcharge for each set of 5 pages in excess of 10: — Company 5,000 DZD" |
| OFF_Renewal | 2nd year (RenMonth ≥ 12) | 5,000 | 5,000 | 1 Jan 2026 | MATCH | DZ-office "762-11 de la 2ème à la 5ème annuité Entreprise ... 5000" |
| OFF_Renewal | 3rd year (RenMonth ≥ 24) | 5,000 | 5,000 | 1 Jan 2026 | MATCH | same |
| (publication fee) | at entry | – | 10,000 (company) | 1 Jan 2026 | MISSING | DZ-office "762-04 Taxe de publication ... Entreprise ... 10000"; DZ "Publication fee: — Company 10,000 DZD" |
| (reduced tariffs) | universities and research centres; start-ups and natural persons | – | e.g. filing 7,000 / 6,500 | 1 Jan 2026 | MISSING | DZ-office "762-01 ... 15000 7000 6500" |

Notes: since 1 January 2026 INAPI charges by applicant category (company / university or research
centre / start-up or natural person). The schedule has no input for these categories (it declares
`EntitySize` but no fee uses it), so it now carries the company rate, the case of a foreign corporate
applicant; the lower tiers are MISSING. The baseline figures (7,500 / 2,000 / 1,200 / 5,000) do not all
belong to one current tier. The eGuide adds "For fees related to patents, a 19% VAT must be added to
the amount of the fee"; the INAPI table does not mention VAT, and VAT is not modelled (see report).

## EA: PCT-EA.ipf (RUB)

Sources: eGuide EA, valid as from August 1, 2026, last updated June 11, 2026 (sources/EA.txt); EAPO "Положение о пошлинах Евразийской патентной организации" (Statute on Fees), current version with amendments of 14–15 October 2025 (sources/EA-office.txt, sources/EA-office.pdf); EAPO news of 14 January 2025 (sources/EA-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-02-01 (Statute: "Вводятся в действие с 1 февраля 2026 г.").
Fees last changed: 1 February 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | no ISR | 36,000 | 70,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "единая процедурная пошлина за подачу евразийской заявки – в размере 70000 рублей"; EA "Unitary procedural fee ... 70,000 RUB" |
| OFF_BasicNationalFee | ISR available (−25%) | 27,000 | 52,500 | 1 Feb 2026 | UPDATED (amount) | EA-office "уменьшаются на 25 процентов"; EA "—25% where an international search report has been established by an Office acting as an International Searching Authority" |
| OFF_ClaimFee | claims 6–20 | 4,800 | 7,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "свыше пятого ... – в размере 7000 рублей"; EA "— in excess of five 7,000 RUB" |
| OFF_ClaimFee | claims 21–50 | 5,200 | 7,500 | 1 Feb 2026 | UPDATED (amount) | EA-office "свыше двадцатого пункта формулы – в размере 7500 рублей" |
| OFF_ClaimFee | claims > 50 | 6,500 | 8,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "свыше пятидесятого пункта формулы – в размере 8000 рублей" |
| OFF_ExaminationFee | one invention | 40,000 | 70,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "(i) в отношении одного изобретения – в размере 70000 рублей" |
| OFF_ExaminationFee | 2nd independent claim | 40,000 | 40,000 | 1 Feb 2026 | MATCH | EA-office "дополнительно 40000 рублей за второй независимый пункт"; EA "additional fee for the second independent claim 40,000 RUB" |
| OFF_ExaminationFee | each independent claim > 2 | 13,000 | 25,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "по 25000 рублей за каждый последующий независимый пункт"; EA "25,000 RUB" |
| OFF_GrantFee | always (optional) | 25,000 | 45,000 | 1 Feb 2026 | UPDATED (amount) | EA-office "5. Выдача евразийского патента ... пошлина в размере 45000 рублей"; EA "Fee for grant of a Eurasian patent 45,000 RUB" |
| (reduction) | ISR established by the EAPO (−40%) | – | 40% | 1 Feb 2026 | MISSING | EA-office "отчета о патентном поиске, проведенном Евразийским ведомством ... уменьшаются на 40 процентов"; the ISA list has no EAPO choice |
| (reduction) | ISR reduction applied to the claim fee | – | 25% / 40% | 1 Feb 2026 | MISSING | EA-office: the reductions apply to "единой процедурной пошлины и пошлины, установленной в абзаце третьем правила 24(7)" (the claim fee); the eGuide names the unitary fee only |
| (reductions) | EAPC-state applicants (90/70/10%); natural persons of listed states (50%) | – | 90/70/10/50% | 1 Feb 2026 | MISSING | EA "reduced by 90% ... by 70% ... by 10% ... and by 50% where ... a natural person who is a national or resident of any of the states listed" |

Notes: the second-independent-claim examination fee used to be written as `Fee` (equal to the basic
examination fee, 40,000 at baseline); with the basic fee now 70,000 the yield is written with the
literal 40,000, unchanged in amount. The baseline 36,000 unitary fee was already superseded by
1 February 2025 at the latest (EA-office-2: "The unitary procedural fee will amount to 60,000 rubles"
as of February 1, 2025). The claim-fee tiers keep the baseline reading (each band at its own rate).
Fees may be paid in USD or EUR at the Central Bank of Russia rate (EA); the schedule keeps RUB.
PCT-EA.ipf declares its own inputs and has no "Inherits from" header; it does not use bases/base_ea.ipf.

## EC: PCT-EC.ipf (USD)

Sources: eGuide EC, valid as from May 28, 2026 (sources/EC.txt). SENADI's table "TASAS SENADI 2026" (derechosintelectuales.gob.ec) could not be reached: connection refused on ports 443 and 80; no archived copy.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-28 (eGuide "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 495.33 | 495.33 | not stated | MATCH | EC "Filing fee ... 495.33 USD" |
| OFF_ClaimFee | per claim > 10 | 55.07 | 55.07 | not stated | MATCH | EC "Additional fee for each claim in excess of 10 55.07 USD" |
| OFF_ExaminationFee | basic | 596.49 | 596.49 | not stated | MATCH | EC "Examination fee (additional 10% for each page in excess of 20) 596.49 USD" |
| OFF_ExaminationFee | per page > 20 (10%) | 59.649 | 59.649 | not stated | MATCH | same |
| OFF_Renewal | 1st year (always) | 125 | 125 | not stated | MATCH | EC "— for the 1st year 125 USD" |
| OFF_Renewal | 2nd year (RenMonth ≥ 12) | 145 | 145 | not stated | MATCH | EC "— for the 2nd year 145 USD" |
| OFF_Renewal | 3rd year (RenMonth ≥ 24) | 158.20 | 158.20 | not stated | MATCH | EC "— for the 3rd year 158.20 USD" |
| (reduction) | MSMEs, national universities, independent researchers, etc. | – | up to 90% | not stated | MISSING | EC "This fee may be reduced by up to 90% for micro, small and medium enterprises ..." |

Notes: matched on the eGuide alone; the office table was unreachable. The search fees (148 / 836.96)
are not national-phase-entry fees in the summary and are not modelled.

## EE: PCT-EE.ipf (EUR)

Sources: eGuide EE, valid as from January 1, 2026, last updated March 27, 2026 (sources/EE.txt); Estonian Patent Office "Fees" page for patents (sources/EE-office.txt, undated).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (eGuide "Valid as from"; the office page states no date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 225 | 225 | not stated | MATCH | EE-office "For accepting of an international patent application for national processing 225"; EE "Basic fee ... 225 EUR (56 EUR)" |
| OFF_ClaimFee | per claim > 10 | 13 | 13 | not stated | MATCH | EE-office "plus 13 euros per claim starting from the eleventh claim"; EE "Claim fee for each claim in excess of 10 ... 13 EUR" |
| OFF_Renewal | years 1–3 (RenMonth ≥ 24) | 116 | 116 | not stated | MATCH | EE "Annual fees for the first three years ... 116 EUR"; EE-office "for the first year of validity 26", "second ... 26", "third ... 64" (sum 116) |
| (reduction) | all applicants natural persons | – | 56 | not stated | MISSING | EE "225 EUR (56 EUR)", "The amount in parentheses is applicable where all applicants are natural persons" |

Notes: none.

## EG: PCT-EG.ipf (EGP)

Sources: eGuide EG, valid as from September 1, 2026, last updated July 23, 2026 (sources/EG.txt); FICPI news item of 7 July 2025 on EGIPA Decree No. 26 of 2025 (sources/EG-secondary.txt, secondary source, used for the date only). The office's site offers only a 2022 scanned decision on support for Egyptian applicants, not a fee table.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-09-01 (eGuide "Valid as from"; neither the office nor the chapter states the effective date of the examination fee).
Fees last changed: 18 June 2025 (examination fee; FICPI, secondary).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 150 | 150 | not stated | MATCH | EG "Filing fee 150 EGP filing by students 0 EGP" |
| OFF_ExaminationFee | always (optional) | 17,000 | 50,000 | 18 Jun 2025 | UPDATED (amount) | EG "Examination fee 50,000 EGP"; EG-secondary "from 25,000 EGP to 50,000 EGP ... The Decree took effect on 18 June 2025" |
| OFF_Renewal | 2nd year (RenMonth ≥ 12) | 20 | 20 | not stated | MATCH | EG "Annual fee for the 2nd year 20 EGP" |
| OFF_Renewal | 3rd year (RenMonth ≥ 24) | 40 | 40 | not stated | MATCH | EG "Annual fee for the 3rd year 40 EGP" |
| (exemptions/reductions) | students (filing fee 0; annual fees 10%); individuals or establishments ≤ 10 staff (annual fees 50%) | – | 0 / 10% / 50% | not stated | MISSING | EG "Students are exempted from paying this fee"; "reduced to 10% for students and ... to 50% for individuals or establishments with a maximum staff of 10 persons" |

Notes: the fee before Decree 26/2025 was 25,000 (EG-secondary), so the baseline 17,000 had already been
superseded once before June 2025; the date of the 17,000 → 25,000 step is not in the saved sources.

## ES: PCT-ES.ipf (EUR)

Sources: eGuide ES, valid as from April 1, 2026 (sources/ES.txt); OEPM "Tasas de invenciones" page (sources/ES-office.txt); OEPM "Patentes y modelos de utilidad" fee table (sources/ES-office-2.txt, undated; its PCT page is "Actualizado 1 de Abril de 2026").
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-01 (eGuide "Valid as from"; the national fee table states no effective date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (electronic) | 87.03 | 87.03 | not stated | MATCH | ES-office-2 "Solicitud de patente o modelo de utilidad IT01 102,39 € IE01 87,03 €"; ES "Online 87.03 EUR" |
| OFF_BasicSearchFee | always (electronic) | 593.65 | 593.65 | not stated | MATCH | ES-office-2 "Solicitud de informe sobre el estado de la técnica (IET). IT04 698,42 € IE04 593,65 €" |
| OFF_BasicExaminationFee | always (electronic) | 337.96 | 337.96 | not stated | MATCH | ES-office-2 "Solicitud de examen sustantivo. IT22 397,61 € IE22 337,96 €" |
| (reduction) | SMEs and entrepreneur natural persons, public universities | – | 43.52 / 296.83 / 168.98 | not stated | MISSING | ES-office-2 "PYMES O PERSONAS FÍSICAS EMPRENDEDORAS ... YE01 43,52 € ... YE04 296,83 € ... YE22 168,98 €" |
| (refund/reduction) | OEPM benefits from the ISR / IPER | – | refund, extent-dependent | not stated | MISSING | ES "refund of search report fee and substantive examination fee can be applicable" |

Notes: the OEPM table also lists a priority-claim fee (IE06, 17.04 € per priority) that the eGuide
does not list as a national-phase fee; not modelled, not counted.

## Summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| CZ | 4 | 3 | 0 | 0 | 0 | 1 | 2026-07-01 (eGuide) | not stated |
| DE | 6 | 5 | 0 | 0 | 0 | 1 | 2026-04-10 (eGuide) | not stated |
| DJ | 1 | 1 | 0 | 0 | 0 | 0 | 2026-01-01 (eGuide) | not stated |
| DK | 5 | 0 | 5 | 0 | 0 | 0 | 2026-01-01 (DKPTO table) | 2026-01-01 (renewals 2026-04-01) |
| DO | 6 | 5 | 0 | 0 | 0 | 1 | 2023-03-31 (ONAPI table) | 2023-03-31 |
| DZ | 7 | 2 | 3 | 0 | 0 | 2 | 2026-01-01 (eGuide change list) | 2026-01-01 |
| EA | 12 | 1 | 8 | 0 | 0 | 3 | 2026-02-01 (EAPO Statute) | 2026-02-01 |
| EC | 8 | 7 | 0 | 0 | 0 | 1 | 2026-05-28 (eGuide) | not stated |
| EE | 4 | 3 | 0 | 0 | 0 | 1 | 2026-01-01 (eGuide) | not stated |
| EG | 5 | 3 | 1 | 0 | 0 | 1 | 2026-09-01 (eGuide) | 2025-06-18 (secondary source) |
| ES | 5 | 3 | 0 | 0 | 0 | 2 | 2026-04-01 (eGuide) | not stated |
| **Total** | **63** | **33** | **17** | **0** | **0** | **13** | | |
