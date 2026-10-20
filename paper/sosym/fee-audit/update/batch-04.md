# Batch 04: FI GB GE GH GM GT HN HR HU ID IL IN

Fees in force on 20 October 2026, compared with the schedules at the start of this pass (IPFees
`beab6fd`, January 2024 baseline). Translation and agent fees were not touched. Amounts in the
"Baseline" and "In force" columns are in the schedule's currency unless marked.

## FI: PCT-FI.ipf (EUR)

Sources: eGuide FI, valid as from 1 April 2026, last updated 26 March 2026 (sources/FI.txt); PRH
price list "Application fees for patents", retrieved 20 October 2026 (sources/FI-office.txt); PRH
notice "Changes to patent, SPC, utility model and integrated circuit layout-design fees from
1 January 2026" (Decree 1216/2025, fees for 1 January to 31 December 2026; sources/FI-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (effective date of the
office's current fee decree, as stated in the PRH notice; the chapter's 1 April 2026 version only
changed ISA/SISA fees).
Fees last changed: 1 January 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (online filing) | 400 | 450 | 1 Jan 2026 | UPDATED (amount); the 2025 fee was 430, so 400 was already out of date before 2026 | FI-office-2 "the online filing system is used €450 (€430)"; FI "for an application filed electronically 450 EUR" |
| OFF_ClaimFee | per claim over 15 | 50 | 60 | 1 Jan 2026 | UPDATED (amount) | FI-office-2 "Additional fee for each claim over 15 €60 (€50)"; FI "Claim fee for each claim in excess of 15 ... 60 EUR" |
| OFF_Renewal | annual fees due at entry: 2nd year from 12 months, 3rd year from 24 months | 200 from 24 months | 80 from 12 months + 140 from 24 months | 1 Jan 2026 | UPDATED (amount, condition) | FI-office-2 "2nd year €80 (€0)", "3rd year €140 (€135)"; FI "Annual fees for the first three years ... 220 EUR" |

Notes: the paper rate of the basic fee (580) is not modelled, as the corpus uses the electronic rate.
Until 2025 the second annual fee was nil; from 1 January 2026 it is €80, so the renewal fee now has
a second yield (`YIELD 80 IF RenMonth GTE 12`) inside the existing OFF_Renewal; no new fee, input
or LET was added. The chapter's "220 EUR for the first three years" is the sum 0 + 80 + 140. The
FI fee decree is annual (1 January to 31 December 2026), so the amounts may change on
1 January 2027.

## GB: PCT-GB.ipf (GBP)

Sources: eGuide GB, valid as from 1 April 2026, last updated 1 May 2026 (sources/GB.txt);
S.I. 2026/183, the Intellectual Property Fees (Miscellaneous Amendments, Revocation and
Transitional Provisions) Rules 2026, in force 1 April 2026 (sources/GB-office.txt); ../AUDIT.md
batch 3 (same figures).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-01 (effective date of
S.I. 2026/183, "come into force on 1st April 2026").
Fees last changed: 1 April 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (Form NP1) | 30 | 40 | 1 Apr 2026 | UPDATED (amount) | GB "National fee ... 40 GBP"; GB-office r.6(b) "for “£30” substitute “£40”" |
| OFF_ClaimFee | per claim over 25 | 20 | 27 | 1 Apr 2026 | UPDATED (amount) | GB "Excess claims fee for each claim in excess of 25 ... 27 GBP"; GB-office r.8 "for “£20” substitute “£27”" |
| OFF_SheetFee | per page of description over 35 | 10 | 13 | 1 Apr 2026 | UPDATED (amount) | GB "Excess pages fee for each page of description over 35 ... 13 GBP"; GB-office r.9 "for “£10” substitute “£13”" |
| OFF_SearchFee | ISR available, electronic request | 120 | 160 | 1 Apr 2026 | UPDATED (amount) | GB "already been made by an International Searching Authority ... 200 GBP — when search/substantive examination is requested electronically 160 GBP" |
| OFF_SearchFee | no ISR, electronic request | 150 | 200 | 1 Apr 2026 | UPDATED (amount) | GB "in other cases: 240 GBP — when search/substantive examination is requested electronically 200 GBP" |
| OFF_ExaminationFee | optional, electronic request | 130 | 130 | 1 Apr 2026 | MATCH (online rate; 130 was the old paper rate, so the match is coincidental) | GB "Substantive examination fee ... 170 GBP — when search/substantive examination is requested electronically 130 GBP"; GB-office "£130 \| £170" |
| Publication of translation (NP1 Part 5 / PA s.89A) | only for applications not published in English | — | 16 | 1 Apr 2026 | MISSING | GB "On request for publication of translation under PA section 89A(3) or (5) 16 GBP"; GB-office r.13(a) "for “£12” substitute “£16”" |

Notes: the comments on the search-fee LETs now read "200-40" and "240-40" (paper fee minus the
GBP 40 electronic reduction, GB-office r.7 "for “£30” substitute “£40”"); a comment "#170-40" was
added to the examination LET. The electronic reduction applies to Forms 9A and 10 only; the NP1
fee is the same on paper and online. No SME or natural-person reductions exist.

## GE: PCT-GE.ipf (USD)

Sources: eGuide GE, valid as from 7 May 2026 (sources/GE.txt); Sakpatenti fee page
https://www.sakpatenti.gov.ge/en/page/264/ (sources/GE-office.txt); Resolution of the Government
of Georgia No 182 of 3 July 2010, consolidated, in Georgian (sources/GE-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2023-06-02 (the office states that
the current table applies to applications "for which the proceeding started on June 2, 2023, or
later").
Fees last changed: 2 June 2023 for the national-phase fees (Resolution No 174 of 23 May 2025 changed
only the reinstatement fee).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always, up to 30 pages, electronic filing | 0.8*90 = 72 | 0.8*100 = 80 | 2 Jun 2023 | UPDATED (amount); before 1 Jan 2024, so the baseline was already out of date | GE-office "Examination as to form and publication of application (up to 30 pages of application materials) 100"; GE "This fee is reduced by 20% for applications filed electronically"; GE-office-2 art. 6¹ "20%-იანი შეღავათი" (electronic filing) |
| OFF_SearchFee | optional, after invitation | 0.8*90 = 72 | not charged | 2 Jun 2023 | REMOVED (the 90 USD item was "Determination of subject-matter of protection" of the pre-June-2023 table; the current table has no such fee) | GE-office old table "Determination of subject-matter of protection 90" vs current table items 1–7 |
| OFF_ExaminationFee | optional, after invitation | 0.8*180 + 0.8*120 per independent claim over 1 | 0.8*300 up to 10 claims + 0.8*30 per claim over 10 | 2 Jun 2023 | UPDATED (amount, basis: all claims instead of independent claims); before 1 Jan 2024 | GE-office "Substantive examination (up to 10 claims) 300", "(for each additional claim over 10) 30"; GE "This fee is reduced by 20% for applications filed electronically" |
| Page fee | each page over 30 | — | 1 USD (0.8 electronic) | 2 Jun 2023 | MISSING | GE-office "(for each additional page over 30) 1"; GE "for each additional page over 30 ... Equivalent in GEL of 1 USD" |
| Examination reduction | ISR (PCT/ISA/210) and written opinion issued | — | 50 % of examination fee | 2 Jun 2023 | MISSING | GE-office "shall be reduced by 50%, if the International Searching Authority has issued International Search Report (form PCT/ISA/210) and Written Opinion" |

Notes: the baseline modelled the table for applications pending on 2 June 2023 (examination as to
form 90, determination of subject-matter 90, state of the art 180 + 120 per extra independent claim),
which is still published on the office page but applies only to old cases. Fees are "payable in
GEL equivalent of US dollar"; the schedule keeps USD. The 50 % ISR reduction applies to almost every
PCT national-phase entry; it is not modelled because the protocol forbids new reductions, so the
schedule overstates the examination fee (240 instead of 120 for ≤10 claims). The 70 %/90 % reductions
for inventors, universities, students and pensioners are not modelled either (the schedule never
modelled them). The IndependentClaimCount input is now unused in this schedule.

## GH: PCT-GH.ipf (GHS)

Sources: eGuide GH, valid as from 1 January 2026, last updated 13 November 2025 (sources/GH.txt).
No office table was retrieved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as
from" date; no office date stated).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always; half for Entity_Person / Entity_Less25 | 100000 / 50000 | 100000 / 50000 | not stated | MATCH | GH "National processing fee 100,000 GHS (50,000 GHS)" |
| OFF_Renewal | 1st year, always | 10000 / 5000 | 10000 / 5000 | not stated | MATCH | GH "for the 1st year 10,000 GHS (5,000 GHS)" |
| OFF_Renewal | 2nd year, from 12 months | 20000 / 10000 | 20000 / 10000 | not stated | MATCH | GH "for the 2nd year 20,000 GHS (10,000 GHS)" |
| OFF_Renewal | 3rd year, from 24 months | 30000 / 15000 | 30000 / 15000 | not stated | MATCH | GH "for the 3rd year 30,000 GHS (15,000 GHS)" |

Notes: the reduced amounts apply to "filing by an individual or by an entity employing less than
25 persons"; the schedule maps this to Entity_Person and Entity_Less25 only (Entity_Micro and
Entity_Small pay full fees), which matches the wording.

## GM: PCT-GM.ipf (GMD)

Sources: eGuide GM, valid as from 13 March 2025 (sources/GM.txt). No office table was retrieved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as
from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 5000 | 5000 GMD (residents) / 400 USD (non-residents) | not stated | MATCH (GMD amount); currency question | GM "Filing fee Includes examination, grant and publication 5,000 GMD or 400 USD"; "The amounts in USD are applicable only for applicants who are not residents of Gambia" |
| OFF_Renewal | 1st year, always | 250 | 250 USD | not stated | UNVERIFIED: the chapter gives annual fees only in USD; the schedule's 250 is the USD figure written as GMD; not converted | GM "for the 1st year 250 USD" |
| OFF_Renewal | 2nd year, from 12 months | 300 | 300 USD | not stated | UNVERIFIED (same reason) | GM "for the 2nd year to the 9th years per year, 300 USD" |
| OFF_Renewal | 3rd year, from 24 months | 300 | 300 USD | not stated | UNVERIFIED (same reason) | GM "for the 2nd year to the 9th years per year, 300 USD" |

Notes: non-residents of Gambia (the typical PCT applicant here) pay the filing fee in USD (400).
The schedule keeps GMD; whether to switch the schedule to USD is for the author to decide.

## GT: PCT-GT.ipf (GTQ)

Sources: eGuide GT, valid as from 13 March 2025 (sources/GT.txt); its Annex GT.I says "Information
not yet available". A web search found no official RPI fee table.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-13 (chapter's "Valid as
from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 2500 | 2500 | not stated | MATCH | GT "Patents Filing fee 2,500 GTQ" |

## HN: PCT-HN.ipf (HNL)

Sources: eGuide HN, valid as from 11 June 2026 (sources/HN.txt). No office table was retrieved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-11 (chapter's "Valid as
from" date).
Fees last changed: not stated (the 11 June 2026 version changed only e-mail addresses).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 1500 | 1500 | not stated | MATCH | HN "Filing fee ... 1,500 HNL" |
| OFF_SearchFee | always | 1000 | 1000 | not stated | MATCH | HN Annex HN.I "Search fee 1,000 HNL" |
| OFF_ExaminationFee | always | 1500 | 1500 | not stated | MATCH | HN "Substantive examination fee 1,500 HNL" |
| OFF_Renewal3 | from 24 months | 200 | 200 per year | not stated | MATCH | HN "Annual fees, due from the international filing date per year, 200 HNL" |
| Inventor hardship reduction | applicant is the inventor and declares inability to pay | — | 90 % reduction of fees | not stated | MISSING | HN "The fees are reduced by 90% where the applicant is the inventor and he provides a declaration" |

Notes: the chapter says annual fees are due "per year" from the international filing date; the
schedule charges a single 200 once 24 months have passed. Whether earlier years are also due at
entry is not stated; the structure was left as it is.

## HR: PCT-HR.ipf (EUR)

Sources: eGuide HR, valid as from 1 January 2026, last updated 1 December 2025 (sources/HR.txt);
Uredba o naknadama za postupke u području intelektualnog vlasništva i stručne usluge DZIV-a,
NN 119/2021, "u primjeni od 13. 11. 2021." (sources/HR-office-2.txt, amounts in kuna); DZIV
"Pregled troškova" for applications filed until 20 February 2020 (sources/HR-office.txt, context
only; it gives the kuna-to-euro rate 7,53450).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2021-11-13 (date from which the
office's current fee regulation applies; the euro amounts in the chapter are its kuna amounts at
the fixed rate 7.53450).
Fees last changed: 13 November 2021 (regulation NN 119/2021); no later change found.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always, up to 30 sheets and 10 claims, electronic filing | 159.27 | 0.5*159.27 = 79.635 | 13 Nov 2021 | UPDATED (basis: electronic rate); before 1 Jan 2024, so the baseline (paper rate) was already off | HR "up to 30 sheets and 10 claims: 159.27 EUR", "Application filed in electronic format: 50% of the applicable fee"; HR-office-2 art. 6(2) "plaćaju se u iznosima umanjenim za 50%" |
| OFF_SheetFee | per sheet over 30, electronic filing | 0.66 | 0.5*0.66 = 0.33 | 13 Nov 2021 | UPDATED (basis) | HR "for each additional sheet: 0.66 EUR"; HR-office-2 art. 6(2) (covers all of art. 6(1), items 1–3) |
| OFF_ClaimFee | per claim over 10, electronic filing | 1.33 | 0.5*1.33 = 0.665 | 13 Nov 2021 | UPDATED (basis) | HR "for each additional claim: 1.33 EUR"; HR-office-2 art. 6(1) item 3 and art. 6(2) |
| OFF_ExaminationFee | optional | 265.45 | 265.45 | 13 Nov 2021 | MATCH (per the chapter; see notes) | HR "Fee for requesting examination on the basis of the request for substantive examination: 265.45 EUR" |
| OFF_Renewal3 | 3rd year, from 24 months | 34.51 | 34.51 | 13 Nov 2021 | MATCH | HR "for the 3rd year: 34.51 EUR"; HR-office-2 art. 14 "za III. godinu 260,00 kn" |
| Inventor reduction | applicant is also the inventor | — | 50 % of filing, examination, publication and maintenance fees | 13 Nov 2021 | MISSING | HR "reduced by 50% where the applicant is also the inventor" |

Notes: the electronic-filing reduction is in the national-phase Summary of the chapter ("The filing
fee is reduced by 50% if the application is filed by electronic means") and, in the regulation,
applies to all three items of art. 6(1); the schedule now carries the electronic rate as the
protocol requires. 265.45 EUR (2,000 kn) is, in the regulation, the fee for the search report
(art. 9(1)); full examination is 1,900 kn = 252.17 EUR (art. 11(1)). The chapter, the more recent
source, calls 265.45 the examination fee, so the amount was kept; the label may be wrong in the
chapter or the schedule.

## HU: PCT-HU.ipf (HUF)

Sources: eGuide HU, valid as from 1 July 2026 (sources/HU.txt); Decree 19/2005 (IV. 12.) GKM,
English consolidated text as at 13.04.2025, section 5 (sources/HU-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-04-13 (date of the office's
consolidated fee decree; the date on which the national fee itself last changed is not stated).
Fees last changed: not stated (the decree was last consolidated on 13 April 2025).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | designated Office | 37400 | 50000 | in force by 13 Apr 2025; change date not stated | UPDATED (amount) | HU-office s.5(2) "Where the Office acts as a designated Office, the national fee is HUF 50 000"; HU "50,000 HUF" |
| OFF_BasicNationalFee | elected Office (0.5 × designated) | 18700 | 25000 | same | UPDATED (amount) | HU-office s.5(3) "Where the Office acts as an elected Office, the national fee is HUF 25 000" |
| OFF_ClaimFee | designated Office, per claim 11–20 / 21–30 / over 30 | 1900 / 3800 / 5600 | 2500 / 5000 / 7500 | same | UPDATED (amount) | HU-office s.5(2) "increased by HUF 2 500 ... HUF 5 000 ... HUF 7 500"; HU Annex HU.I |
| OFF_ClaimFee | elected Office (0.5 × designated) | 950 / 1900 / 2800 | 1250 / 2500 / 3750 | same | UPDATED (amount) | HU-office s.5(3) "increased by HUF 1 250 ... HUF 2 500 ... HUF 3 750" |

Notes: the elected-Office amounts remain exactly half the designated-Office amounts, so the
schedule's 0.5 factor still holds. Not modelled and not national-phase entry fees in the ordinary
case: the surcharge of HUF 79,000 for late translation or late payment, and the national search
surcharge (half the national fee) where parts of the application were not searched (s.5(6)). The
examination fee (86,000) falls due later in the national procedure.

## ID: PCT-ID.ipf (IDR)

Sources: eGuide ID, valid as from 12 November 2024, last updated 27 March 2026 (sources/ID.txt). No
office table was retrieved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2024-11-12 (chapter's "Valid as
from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always, electronic filing | 1250000 | 1250000 | not stated | MATCH | ID "Filing fee electronic 1,250,000 IDR on paper 1,500,000 IDR" |
| OFF_SheetFee | per page of description over 30 | 15000 | 15000 | not stated | MATCH | ID "for each page of the description in excess of 30 15,000 IDR" |
| OFF_ClaimFee | per claim over 10 | 75000 | 75000 | not stated | MATCH | ID "Claim fee for each claim in excess of 10 75,000 IDR" |
| OFF_ExaminationFee | optional | 3000000 | 3000000 | not stated | MATCH | ID "Substantive examination fee 3,000,000 IDR" |

Notes: the "Additional fee for late entry into national phase" (5,000,000) applies only to late
entry and is not modelled.

## IL: PCT-IL.ipf (ILS)

Sources: eGuide IL, valid as from 1 August 2026, last updated 11 June 2026 (sources/IL.txt);
../AUDIT.md batch 3 (Israel Patent Office pages returned HTTP 403; same amounts).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-08-01 (chapter's "Valid as
from" date; the office's own effective date for the 2026 indexation could not be established).
Fees last changed: 2026 indexation; date not established (sources conflict between 1 January and
1 March 2026, per AUDIT.md).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 2183 | 2402 | 2026 (date not established) | UPDATED (amount) | IL "Filing fee 2,402 ILS" |
| OFF_SheetFee | per 50 sheets (or part) over 100 | 273 | 300 | same | UPDATED (amount) | IL "Application size fee for each additional 50 sheets that exceed 100 sheets 300 ILS" |
| OFF_ClaimFee | per claim from the 51st | 560 | 616 | same | UPDATED (amount) | IL "Claim fee for the 51st and each subsequent claim 616 ILS" |

Notes: the 40 % reduction applies to "applications first filed in Israel" by individuals or
companies with turnover under ILS 10 million; a PCT national-phase entry is normally not first filed
in Israel, so it is not counted as MISSING. The chapter's Summary says "None" for reductions.

## IN: PCT-IN.ipf (INR)

Sources: eGuide IN, valid as from 1 July 2026, last updated 25 September 2026 (sources/IN.txt);
../AUDIT.md batch 2 (IP India Schedule 1, undated; same amounts).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-01 (chapter's "Valid as
from" date; Schedule 1 carries no effective date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | up to 30 sheets and 10 claims, e-filing; small/person vs other | 1600 / 8000 | 1600 / 8000 | not stated | MATCH | IN "electronic filing 1,600 INR ... electronic filing 8,000 INR" |
| OFF_SheetFee | per sheet over 30 | 160 / 800 | 160 / 800 | not stated | MATCH | IN "For each additional sheet in addition to 30 ... electronic filing 160 INR ... electronic filing 800 INR" |
| OFF_ClaimFee | per claim over 10 | 320 / 1600 | 320 / 1600 | not stated | MATCH | IN "For each claim in addition to 10 ... electronic filing 320 INR ... electronic filing 1,600 INR" |
| OFF_PriorityFee | per priority after the first | 1600 / 8000 | 1600 / 8000 | not stated | MATCH | IN "For each additional priority, multiple of ... 1,600 INR ... 8,000 INR" |
| OFF_ExaminationFee | optional (Form 18, s.11B) | 4000 / 20000 | 4000 / 20000 | not stated | MATCH | IN "under Section 11(B) Electronic filing ... 4,000 INR ... 20,000 INR" |
| Examination request under Rule 20(4)(ii) | PCT national-phase applications | — | 5600 / 28000 | not stated | MISSING | IN "under Rule 20(4)(ii) Electronic filing ... 5,600 INR ... 28,000 INR" |
| Sequence-listing pages | per page, Rule 9(3) | — | 160 / 800, capped at 24,000 / 120,000 | not stated | MISSING | IN "each page of sequence listing ... 160 INR Subject to a maximum of 24,000 INR ... 800 INR Subject to a maximum of 120,000 INR" |

Notes: the comment on OFF_ExaminationFee ("48 months of priority") was corrected to 31 months
(eGuide IN.12 "within 31 months from the date of priority"). The reduced rate covers "natural
person(s) or startup(s) or small entity(ies) or educational institution(s)"; the schedule reduces
only Entity_Small and Entity_Person, so Entity_Micro (an Indian micro enterprise is a small entity)
pays the full rate. The condition was left unchanged because the corpus categories do not map
one-to-one onto the Indian ones.

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| FI | 3 | 0 | 3 | 0 | 0 | 0 | 2026-01-01 (office decree) | 1 Jan 2026 |
| GB | 7 | 1 | 5 | 0 | 0 | 1 | 2026-04-01 (S.I. 2026/183) | 1 Apr 2026 |
| GE | 5 | 0 | 2 | 1 | 0 | 2 | 2023-06-02 (office table) | 2 Jun 2023 |
| GH | 4 | 4 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| GM | 4 | 1 | 0 | 0 | 3 | 0 | 2025-03-13 (chapter) | not stated |
| GT | 1 | 1 | 0 | 0 | 0 | 0 | 2025-03-13 (chapter) | not stated |
| HN | 5 | 4 | 0 | 0 | 0 | 1 | 2026-06-11 (chapter) | not stated |
| HR | 6 | 2 | 3 | 0 | 0 | 1 | 2021-11-13 (office regulation) | 13 Nov 2021 |
| HU | 4 | 0 | 4 | 0 | 0 | 0 | 2025-04-13 (office decree, consolidated) | not stated (decree consolidated 13 Apr 2025) |
| ID | 4 | 4 | 0 | 0 | 0 | 0 | 2024-11-12 (chapter) | not stated |
| IL | 3 | 0 | 3 | 0 | 0 | 0 | 2026-08-01 (chapter) | 2026 indexation, date not established |
| IN | 7 | 5 | 0 | 0 | 0 | 2 | 2026-07-01 (chapter) | not stated |
| **All** | **53** | **22** | **20** | **1** | **3** | **7** | | |
