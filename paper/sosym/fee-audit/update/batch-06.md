# Batch 06: KR KW KZ LA LK LR LS LU MA MD MG MK

Checked on 20 October 2026 against the saved sources in `sources/`. Edits are confined to the
twelve schedules `PCT-XX.ipf`: in each, the version line and the `# Valid until n/a` comment; in KR
and KZ also amounts. No fee, input, condition, reduction or `VERIFY` directive was added or removed.

## KR: PCT-KR.ipf (KRW)

Sources: eGuide KR, valid as from 1 August 2026 (sources/KR.txt); MOIP English fee page, "The fees have been in effect since August 1, 2023", page "Last updated 10 February 2026" (sources/KR-office.txt); ../AUDIT.md, batch 1, KR (19 October 2026).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2023-08-01 (effective date stated by the office's fee page).
Fees last changed: 1 August 2023.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | electronic filing | 46,000 | 46,000 | 1 Aug 2023 | MATCH | KR "in electronic form 46,000 KRW"; KR-office "Patent Application (Electronic) 46,000" |
| OFF_BasicNationalFee | −70 % natural person | −70 % | −70 % (natural person who is also the inventor; only 30 % above 20 applications a year) | not stated | MATCH (condition simplified) | KR "reduced by 70% where the applicant is a natural person and is also the inventor" |
| OFF_PriorityFee | per priority claim, electronic | 18,000 | 18,000 | 1 Aug 2023 | MATCH | KR "for each priority claim in electronic form 18,000 KRW"; KR-office "18,000(electronic)" |
| OFF_ExaminationFee | basic + per claim | 143,000 + 44,000/claim | 166,000 + 51,000/claim | 1 Aug 2023 (baseline already out of date on 1 Jan 2024) | UPDATED (amount) | KR "Fee for request for examination 166,000 KRW ... For each claim plus 51,000 KRW"; KR-office "Request for substantive examination ... 166,000 51,000" |
| OFF_ExaminationFee | −70 % natural person | −70 % | −70 % (as above) | not stated | MATCH (condition simplified) | KR "Filing fee, fee for request for examination ... are reduced by 70%" |
| Examination fee reduction | ISR and/or IPER established by MOIP | — | −70 % | not stated | MISSING | KR "reduced by 70% where the international search report and/or the international preliminary examination report has been established by" MOIP |

Notes: Confirms AUDIT.md (KR exam fee stale since 1 August 2023; KR ISR/IPER reduction missing). The
paper rates (66,000 national fee plus 1,000 per sheet over 20; 20,000 per priority) are not modelled
because the corpus uses the electronic rate; AUDIT.md counted the paper national fee as MISSING. The
office's priority row reads "a. Basic fee / b. Additional fee for each priority claim" with 18,000 on
both lines; the eGuide states one fee "for each priority claim", which the schedule models. The
natural-person reduction is applied to any `Entity_Person`; the office limits it to a natural person
who is also the inventor, and to 30 % (filing fee) for more than 20 applications a year. The 1st-3rd
year annual fees (13,000 + 12,000/claim per year) fall due after the decision to grant and are not
national-phase-entry fees.

## KW: PCT-KW.ipf (KWD)

Sources: eGuide KW, valid as from 13 March 2025 (sources/KW.txt); Annex KW.I reads "Information not yet available". No office table was found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | company (not Entity_Person) | 80 | 80 | not stated | MATCH | KW "filing by a company 80 KWD" |
| OFF_BasicNationalFee | individual (Entity_Person) | 40 | 40 | not stated | MATCH | KW "filing by an individual 40 KWD" |
| Filing fee reduction | applicant is a student | — | −50 % | not stated | MISSING | KW "The filing fee is reduced by 50% where the applicant is a student" |

Notes: The chapter's latest changes (13 March 2025) concern reinstatement and restoration of priority, not fees.

## KZ: PCT-KZ.ipf (KZT)

Sources: eGuide KZ, valid as from 1 October 2026 (sources/KZ.txt); Qazpatent invention fee page, no date stated, prices "in tenge, including VAT" (sources/KZ-office-2.txt; fee index page sources/KZ-office.txt); AIPPI news item of 13 March 2026, "Effective 1 January 2026, Kazakhstan's National Institute of Intellectual Property (Qazpatent) implemented moderate fee increases" (sources/KZ-office-3.txt, secondary).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-10-01 (chapter's "Valid as from" date; neither the office page nor the chapter states the effective date of the tariff).
Fees last changed: 1 January 2026 (Qazpatent tariff, per the AIPPI item; secondary source).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (legal entity) | 20,320.16 | 21,045.88 | 1 Jan 2026 (AIPPI) | UPDATED (amount) | KZ "Filing fee and fee for formal examination ... 21,045.88 KZT"; KZ-office-2 "formal examination of an invention: 21 045,88" |
| OFF_ExaminationFee | one invention (legal entity) | 66,959.20 | 69,350.60 | 1 Jan 2026 (AIPPI) | UPDATED (amount) | KZ "— for one invention 69,350.60 KZT"; KZ-office-2 "Substantive examination of an invention application 69 350,60" |
| OFF_ExaminationFee | −20 % if ISR or IPER established | −20 % | −20 % | not stated | MATCH | KZ "The examination fee is reduced by 20% where an international search report or an international preliminary examination report has been established" |
| OFF_ExaminationFee | each independent claim / invention in excess of one | 53,519.20 | 55,430.60 | 1 Jan 2026 (AIPPI) | UPDATED (amount) | KZ "— for each invention in excess of one ... 55,430.60 KZT"; KZ-office-2 "for each independent clause in excess of one 55 430,60" |
| OFF_ExaminationFee | reduction of the excess-invention fee if ISR or IPER established | −15 % | 20 %? | — | UNVERIFIED | No saved source gives 15 %. The chapter attaches "Reduced by 20% where an international search report or an international preliminary examination report has been established" to the excess-invention line, but its layout does not settle whether the note covers that line; the office page gives no reduction. Left at 0.15. |
| Entity reductions | SME / individual / privileged applicants | — | SME 16,836.70 and 55,480.48; individual 6,313.76 and 20,805.18 (filing and exam) | 1 Jan 2026 | MISSING | KZ-office-2 "For small and medium-sized businesses ... For individuals" |
| Annual fees, years 1-3 | per year | — | 21,045.88 | — | MISSING | KZ "Annual fees for the first three years, per year 21,045.88 KZT" (listed under national fees; due date not stated) |

Notes: All three amounts rose by the same factor (about 3.57 %). The office prices include VAT; the
eGuide amounts are identical. The chapter's change list for 1 October 2026 shows these fees as
changed; the office page does not date its tariff, so the effective date 1 January 2026 rests on the
AIPPI item. The grant/publication fee (34,441.56) is due at grant. The schedule is composed with
`bases/base_ea.ipf` (read-only, not checked here).

## LA: PCT-LA.ipf (LAK)

Sources: eGuide LA, valid as from 1 April 2025 (sources/LA.txt). No office table was found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-04-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 200,000 | 200,000 | not stated | MATCH | LA "Filing fee 200,000 KIP" |
| OFF_ExaminationFee | always | 100,000 | 100,000 | not stated | MATCH | LA "Formality examination fee 100,000 KIP" |
| Consultation fee | always | — | 100,000 | not stated | MISSING | LA "Consultation fee 100,000 KIP" |
| Publication fee | always | — | 400,000 | not stated | MISSING | LA "Publication fee 400,000 KIP" |
| Service fee | always | — | 300,000 | not stated | MISSING | LA "Service fee 300,000 KIP" |

Notes: The schedule's comment says the three missing fees were left out "as unknown when these are
payable"; the chapter now lists all five under national fees that "Must be paid within the time limit
applicable under PCT Article 22 or 39(1)". KIP is the eGuide's name for the LAK.

## LK: PCT-LK.ipf (LKR)

Sources: eGuide LK, valid as from 4 June 2026 (sources/LK.txt); NIPO fee list PDF, undated (sources/LK-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-04 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | others (not Entity_Person) | 6,900 | 6,900 | not stated | MATCH | LK "Others 6,900 LKR" |
| OFF_BasicNationalFee | natural person | 2,875 | 2,875 | not stated | MATCH | LK "Natural persons 2,875 LKR" |
| Application fee, students | students | — | 1,150 | not stated | MISSING | LK "Students 1,150 LKR" |

Notes: The NIPO PDF on the office site is older (patent application fee "Individuals 2500.00 ... Others
6000.00", with a 12 % VAT column); the eGuide's amounts are 15 % higher and more recent, so the eGuide
is preferred. The fees are subject to VAT of 15 %, which the schedule (like the baseline) does not add.

## LR: PCT-LR.ipf (USD)

Sources: eGuide LR, valid as from 1 January 2026 (sources/LR.txt). No office table was found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated (the chapter's 1 January 2026 changes concern RO fees only).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 400 | 400 | not stated | MATCH | LR "Filing fee 400 USD"; Annex "Fee for national processing 400 USD" |
| OFF_Renewal | 1st year (always) | 40 | 40 | not stated | MATCH | LR "for the 1st year 40 USD" |
| OFF_Renewal | 2nd year (≥ 12 months) | 60 | 60 | not stated | MATCH | LR "for the 2nd year 60 USD" |
| OFF_Renewal | 3rd year (≥ 24 months) | 80 | 80 | not stated | MATCH | LR "for the 3rd year 80 USD" |

Notes: The annex also lists a publication fee (150 USD) and a grant fee (10 USD); the summary lists
only the filing fee as due at entry, so they are not counted as MISSING. The schedule's comment
"Examination fee is due but unknown" has no counterpart in the chapter, which lists no examination
fee. The schedule is composed with `bases/base_ap.ipf` (read-only).

## LS: PCT-LS.ipf (LSL)

Sources: eGuide LS, valid as from 5 January 2026 (sources/LS.txt). No office table was found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-05 (chapter's "Valid as from" date).
Fees last changed: not stated (the chapter's 5 January 2026 change concerns RO electronic filing).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | standard / individual or small entity | 250/100 | 250/100 | not stated | MATCH | LS "Application fee 250 LSL (100 LSL)" |
| OFF_Renewal | 2nd year (≥ 12 months), standard / reduced | 100/25 | 100/25 | not stated | MATCH | LS "for the 2nd year 100 LSL (25 LSL)" |
| OFF_Renewal | 3rd year (≥ 24 months), standard / reduced | 250/100 | 250/100 | not stated | MATCH | LS "for the 3rd year 250 LSL (100 LSL)" |
| Grant and publication fee | standard / reduced | — | 450/120 | not stated | MISSING | LS "Grant and publication fee ... 450 LSL (120 LSL)" (listed under national fees; "the Office should be consulted for the time limit") |

Notes: The reduced rate applies to individuals and to "small entities" (business concerns of up to
500 employees that have not assigned or licensed the right); the schedule gives it to
`Entity_Person` and `Entity_Small` only, not to `Entity_Micro`, `Entity_Less25` or `Entity_Less100`,
which would also qualify. Left unchanged. The schedule is composed with `bases/base_ap.ipf` (read-only).

## LU: PCT-LU.ipf (EUR)

Sources: eGuide LU, valid as from 24 April 2026 (sources/LU.txt); OPI "Annexe 1 – Barème des taxes en matière de brevets d'invention en vigueur", PDF published April 2026, no effective date stated (sources/LU-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-24 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 40 | 40 | not stated | MATCH | LU "Filing fee ... 40 EUR"; LU-office "Taxe de dépôt (brevet national) : € 40,00" |
| OFF_Renewal3 | 3rd year (≥ 24 months) | 33 | 33 | not stated | MATCH | LU "3rd annual fee ... 33 EUR"; LU-office "3ème année : € 33,00" |
| Filing fee exemption | international application filed with LU as RO | — | no filing fee | not stated | MISSING | LU "No filing fee is payable if the international application was filed with the Intellectual Property Office (Luxembourg) as receiving Office" |

Notes: The OPI table also lists a search fee (€450) and an early-publication fee (€49) for national
applications; the chapter does not list them for the national phase.

## MA: PCT-MA.ipf (MAD)

Sources: eGuide MA, valid as from 25 April 2024 (sources/MA.txt); OMPIC English tariffs page (sources/MA-office.txt), which only cites "decision No 06/2012 of 21 June 2012 applicable from the 1st October 2012" and links a 2012 price list (not saved).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2024-04-25 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | standard / reduced (online) | 750/250 | 750/250 | not stated | MATCH | MA "Filing fee ... 1,000 MAD (750 MAD) 500 MAD ... (250 MAD)" (parentheses: online filing) |
| OFF_PublicationFee | standard / reduced (online) | 750/250 | 750/250 | not stated | MATCH | MA "Publication fee ... 1,000 MAD (750 MAD) 500 MAD ... (250 MAD)" |
| OFF_ClaimFee | per claim over 10, standard / reduced | 400/160 | 400/160 | not stated | MATCH | MA "Additional fee for publication of claims in excess of 10 per claim 400 MAD per claim 160 MAD" |
| Search report fee | standard / reduced (online) | — | 6,000/2,000 | not stated | MISSING | MA "Fee for establishment of the search report with opinion on patentability 8,000 MAD (6,000 MAD) 4,000 MAD ... (2,000 MAD)" |

Notes: Fees are subject to 20 % VAT, not added (as at baseline). The reduced rate applies to natural
persons (from countries benefiting from the PCT fee reduction) and to Moroccan SMEs, universities and
similar national applicants; the schedule gives it to any `Entity_Person`, `Entity_Small` or
`Entity_Micro`, which overstates it for foreign SMEs and for natural persons from other countries.
Left unchanged.

## MD: PCT-MD.ipf (EUR)

Sources: eGuide MD, valid as from 1 June 2026 (sources/MD.txt); AGEPI "Inventions. Fees" (Government Decision No. 774 of 13 August 1997, as amended; no effective date stated) (sources/MD-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 100 | 100 | not stated | MATCH | MD "Filing fee ... 100 EUR"; MD-office "Opening of the national phase ... 100" |
| OFF_PriorityFee | at least one priority (flat) | 100 | 100 | not stated | MATCH | MD "Fee for claiming priority 100 EUR"; MD-office "Claiming conventional or exhibition priority ... 100" |
| OFF_ExaminationFee | basic | 400 | 400 | not stated | MATCH | MD "Substantive examination, including search: 400 EUR" |
| OFF_ExaminationFee | each independent claim over 1 | 100 | 100 | not stated | MATCH | MD "for each independent claim ... in excess of one 100 EUR" |
| OFF_ExaminationFee | each dependent claim over 5 | 10 | 10 | not stated | MATCH | MD "for each dependent claims ... in excess of five 10 EUR" |
| OFF_ExaminationFee | each sheet over 15 | 10 | 10 | not stated | MATCH | MD "fee per sheet in excess of 15 10 EUR" |
| Filing claim fee | each claim over 5 | — | 20 | not stated | MISSING | MD-office "b) for each claim, whether original or amended, exceeding 5 20" (not in the eGuide) |
| Annual fees, years 1-5 | per year | — | 100 | not stated | MISSING | MD "Annual fee for the 1st to the 5th year, per year 100 EUR" (listed under national fees) |
| Reductions | natural person sole author; science organisation; SME or other natural person | — | 95 % / 95 % / 85 % "of the prescribed amount" | not stated | MISSING | MD-office "95% of the prescribed amount of fee, if it is a natural person and is the sole author" |

Notes: Neither source says whether the priority fee is per priority; the schedule's flat fee is kept.
The office's reduction notes read "granted discounts on fees as follows: 95% of the prescribed amount
of fee", which does not make clear whether 95 % is the discount or the amount payable. Since 1 June
2026 a European patent can also be obtained for Moldova for international applications filed on or
after that date (OJ EPO 2026, A25); the national route modelled here is unaffected.

## MG: PCT-MG.ipf (MGA)

Sources: eGuide MG, valid as from 7 May 2026 (sources/MG.txt); OMAPI "Barème des taxes de propriété industrielle (Arrêté n° 12285/2013)" (sources/MG-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-07 (chapter's "Valid as from" date; the office table gives only the year of its order, 2013).
Fees last changed: 2013 (Arrêté n° 12285/2013; day not stated).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 180,000 | 180,000 | 2013 | MATCH | MG "Filing fee, including the fee for grant and publication ... 180,000 MGA"; MG-office "Taxe de dépôt ... 180 000" |
| OFF_ClaimFee | per claim over 10 | 60,000 | 60,000 | 2013 | MATCH | MG "Claim fee for each claim in excess of ten 60,000 MGA" |
| OFF_SheetFee | per sheet over 20 | 25,000 | 25,000 | 2013 | MATCH | MG "Additional fee for each sheet of the description, in excess of twenty 25,000 MGA" |
| OFF_PriorityFee | per priority | 65,000 | 65,000 | 2013 | MATCH | MG "Fee for priority claims, per priority claim 65,000 MGA" |
| OFF_Renewal3 | 3rd year (≥ 24 months) | 125,000 | 125,000 | 2013 | MATCH | MG "for the 3rd year: 125,000 MGA" |
| Reduced rates | applicant from the States listed in the order | — | 144,000 / 48,000 / 20,000 / 52,000 / 100,000 | 2013 | MISSING | MG "The amount in parentheses is applicable if the applicant is a natural person who resides in, or a legal entity whose head office is in, one of the States specified" |

Notes: The sheet fee applies to sheets "of the description"; the schedule counts all pages
(`SheetCount`), as at baseline.

## MK: PCT-MK.ipf (MKD)

Sources: eGuide MK, valid as from 13 March 2025 (sources/MK.txt). No office table was saved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 800 | 800 | not stated | MATCH | MK "Filing fee 800 MKD" |

Notes: The annex also lists a publication fee (1,500 MKD) and maintenance fees from the 3rd year
(800 MKD); the summary lists only the filing fee as due at entry, so neither is counted as MISSING.
The 3rd-year maintenance fee is not modelled, unlike the neighbouring schedules that model the
3rd-year renewal (LU, MG).

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| KR | 6 | 4 | 1 | 0 | 0 | 1 | 2023-08-01 (office) | 1 Aug 2023 |
| KW | 3 | 2 | 0 | 0 | 0 | 1 | 2025-03-13 (chapter) | not stated |
| KZ | 7 | 1 | 3 | 0 | 1 | 2 | 2026-10-01 (chapter) | 1 Jan 2026 (AIPPI) |
| LA | 5 | 2 | 0 | 0 | 0 | 3 | 2025-04-01 (chapter) | not stated |
| LK | 3 | 2 | 0 | 0 | 0 | 1 | 2026-06-04 (chapter) | not stated |
| LR | 4 | 4 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| LS | 4 | 3 | 0 | 0 | 0 | 1 | 2026-01-05 (chapter) | not stated |
| LU | 3 | 2 | 0 | 0 | 0 | 1 | 2026-04-24 (chapter) | not stated |
| MA | 4 | 3 | 0 | 0 | 0 | 1 | 2024-04-25 (chapter) | not stated |
| MD | 9 | 6 | 0 | 0 | 0 | 3 | 2026-06-01 (chapter) | not stated |
| MG | 6 | 5 | 0 | 0 | 0 | 1 | 2026-05-07 (chapter) | 2013 |
| MK | 1 | 1 | 0 | 0 | 0 | 0 | 2025-03-13 (chapter) | not stated |
| **Total** | **55** | **35** | **4** | **0** | **1** | **15** | | |
