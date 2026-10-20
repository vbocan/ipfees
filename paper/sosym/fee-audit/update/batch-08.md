# Batch 08: PE PG PH PL PT QA RO RS RU SA SC SD

Checked on 20 October 2026 against the fees in force that day. Baseline: IPFees `beab6fd`
(January 2024 amounts). Schedules edited: `ipfees/src/IPFLang.Engine/Corpus/PCT-<code>.ipf` for
the twelve offices below; nothing else touched. Rows include MISSING rows; translation and agent
fees and explicit zero cases are not rows.

## PE: PCT-PE.ipf (PEN)

Sources: eGuide PE, valid as from January 1, 2026 (sources/PE.txt). The INDECOPI fee table (TUPA)
could not be reached as an official page; secondary sites give conflicting UIT-based figures and
were not used.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 720 | 720 | not stated | MATCH | PE.txt "Filing fee ... 720 PEN" |
| OFF_ClaimFee | per claim above 10 | 23.43 | 23.43 | not stated | MATCH | PE.txt "Additional fee for each claim in excess of 10 23.43 PEN" |
| OFF_PriorityFee | per priority | 134.49 | 134.49 | not stated | MATCH | PE.txt "Priority claim fee per priority 134.49 PEN" |
| OFF_Renewal | annual fee, 1st year to grant | 140 | 140 | not stated | MATCH | PE.txt "for the 1st year to grant 140 PEN (172 PEN)" |

Notes: the examination fee (549.13 PEN) is due within six months of publication, not at entry,
and is not counted as missing. Utility-model fees are out of scope.

## PG: PCT-PG.ipf (PGK)

Sources: eGuide PG, valid as from January 1, 2026 (sources/PG.txt). No office fee page found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 1000 | 1000 | not stated | MATCH | PG.txt "Filing fee: ... 1,000 PGK" |
| OFF_Renewal | 2nd year (from 12 months) | 170 | 170 | not stated | MATCH | PG.txt "Annual fee for the 2nd year 170 PGK" |
| OFF_Renewal | 3rd year (from 24 months) | 200 | 200 | not stated | MATCH | PG.txt "— for the 3rd year 200 PGK" |
| Filing fee with sequence listing | application includes sequence listings on electronic media | — | 1,500 | not stated | MISSING | PG.txt "if the application includes nucleotide and/or amino acid sequence listings ... 1,500 PGK" |

Notes: the chapter does not say whether the 1,500 PGK replaces or adds to the 1,000 PGK filing fee;
the schedule has no sequence-listing input, so it is left out.

## PH: PCT-PH.ipf (PHP)

Sources: eGuide PH, valid as from January 1, 2026 (sources/PH.txt). The IPOPHL schedule-of-fees
pages are behind a bot-protection wall (Incapsula) and could not be saved.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | big / small entity | 4320/2000 | 4320/2000 | not stated | MATCH | PH.txt "Filing fee ... big entity 4,320 PHP small entity 2,000 PHP" |
| OFF_PriorityFee | per priority, big / small | 2160/1000 | 2160/1000 | not stated | MATCH | PH.txt "Request for right of priority big entity 2,160 PHP small entity 1,000 PHP" |
| OFF_ClaimFee | per claim above 5, big / small | 360/180 | 360/180 | not stated | MATCH | PH.txt "For each claim in excess of five (5) big entity 360 PHP small entity 180 PHP" |
| OFF_SheetFee | per sheet above 30, big / small | 36/18 | 36/18 | not stated | MATCH | PH.txt "For each sheet in excess of thirty (30) big entity 36 PHP small entity 18 PHP" |
| Late national-phase entry fee | entry after 30 and up to 31 months | — | 2,160/1,000 | not stated | MISSING | PH.txt "Additional fee for late entry into the national phase big entity 2,160 PHP small entity 1,000 PHP" |
| Filing-fee exemption | priority of an earlier Philippine national application claimed | — | 0 | not stated | MISSING | PH.txt "No filing fee is payable if priority of an earlier national application is claimed" |

Notes: the chapter defines a small entity by assets (100 million PHP or less), not by being a
natural person; the baseline condition (Entity_Small or Entity_Person) is kept. Substantive
examination (4,200/2,010 PHP) and annual fees from the 5th year are not due at entry.

## PL: PCT-PL.ipf (PLN)

Sources: eGuide PL, valid as from August 1, 2026 (sources/PL.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-08-01 (chapter "Valid as from").
Fees last changed: not stated (the August 2026 change list concerns Annex C only).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | IPRP (Chapter II) established | 350 | 350 | not stated | MATCH | PL.txt "where an international preliminary examination has been carried out 350 PLN" |
| OFF_BasicNationalFee | no IPRP | 550 | 550 | not stated | MATCH | PL.txt "where no international preliminary examination has been carried out 550 PLN" |
| OFF_SheetFee | per sheet above 20 | 25 | 25 | not stated | MATCH | PL.txt "additional fee for each sheet in excess of 20 25 PLN" |
| OFF_PriorityFee | per priority | 100 | 100 | not stated | MATCH | PL.txt "Fee for priority claims per priority 100 PLN" |

Notes: the annual fee for the first three years (480 PLN) is paid on invitation after grant and is
not counted as missing.

## PT: PCT-PT.ipf (EUR)

Sources: eGuide PT, valid as from July 1, 2026 (sources/PT.txt); INPI Deliberação n.º 334/2026,
DR 2.ª série n.º 56 of 20 March 2026, "com efeitos a 1 de julho de 2026" (sources/PT-office.txt,
saved from WIPO Lex); for comparison, INPI Deliberação 447/2024, effective 1 July 2024
(sources/PT-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-01 (office table's effective date, which is also the chapter's).
Fees last changed: 1 July 2026 (INPI updates its fees every 1 July).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | online | 54.54 | 64.21 | 1 Jul 2026 | UPDATED (amount) | PT-office "Pedido internacional de patente (PCT) ... Entrada em fase nacional 64,21 € 128,42 €" (online / paper) |
| Late-entry surcharge | translation or fee within one month after the time limit | — | 50% of the filing fee | not stated | MISSING | PT.txt "subject to the payment of a surcharge equal to 50% of the filing fee" |

Notes: **source conflict.** The chapter gives the national-phase fee as the national filing fee,
"Filing fee (includes publication and examination): online 128.42 EUR", which is the item "Patente
Nacional, Pedido 128,42 €" of the same INPI table. The INPI table also has a specific item for PCT
applications, "Entrada em fase nacional 64,21 €" online. The baseline 54.54 is clearly that PCT
item at an earlier year's level (2024: 61,51 € online, PT-office-2), so the schedule keeps that
basis and now carries 64.21. Both sources take effect on 1 July 2026, so recency does not decide;
if INPI in fact charges PCT entrants the full national filing fee, as the chapter now states, the
amount should be 128.42. Decision needed. The 2024 table already had 61.51, so the baseline was
out of date when the corpus was built (the earlier date of 54.54 is not established).

## QA: PCT-QA.ipf (QAR)

Sources: eGuide QA, valid as from June 26, 2026 (sources/QA.txt). The Office has no website
("Website: None").
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-26 (chapter "Valid as from"; the office's own effective date is not stated).
Fees last changed: not stated; the new filing amounts are marked as changed in the chapter version of 26 June 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | companies / individuals | 2000/1000 | 5000/2500 | not stated (in the chapter from 26 Jun 2026) | UPDATED (amount, basis) | QA.txt "Filing fee and substantive examination fee — in case of filing by companies, institutions and research entities 5,000 QAR — in case of filing by individuals 2,500 QAR" |
| OFF_Renewal | 2nd year (from 12 months), companies / individuals | 400/200 | 400/200 | not stated | MATCH | QA.txt "for the 2nd year ... 400 QAR ... individuals 200 QAR" |
| OFF_Renewal | 3rd year (from 24 months), companies / individuals | 500/250 | 500/250 | not stated | MATCH | QA.txt "for the 3rd year ... 500 QAR ... individuals 250 QAR" |

Notes: the national fee is now a combined filing and substantive-examination fee; the schedule's
single basic fee carries it (basis change). The grant and publication fee (1,600/800 QAR) is due
after grant notification.

## RO: PCT-RO.ipf (EUR)

Sources: ../AUDIT.md, RO section (OSIM Order 79/01.10.2025, Annex 1, applicable from 1 January
2026); eGuide RO, valid as from January 1, 2026 (sources/RO.txt), which agrees with it on every
modelled amount.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (office table's effective date, also the chapter's).
Fees last changed: lei amounts indexed on 1 January 2026; the EUR column, which foreign applicants pay, is unchanged from the 2025 annex (AUDIT.md).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 30 | 30 | 1 Jan 2026 (unchanged) | MATCH | RO.txt "Filing fee 30 EUR"; AUDIT "30 (152 lei)" |
| OFF_PriorityFee | per priority | 50 | 50 | 1 Jan 2026 (unchanged) | MATCH | RO.txt "Fee for claiming priority 50 EUR" |
| OFF_ExaminationFee | paid within 3 months of entry | 500 | 500 | 1 Jan 2026 (unchanged) | MATCH | RO.txt "paid within three months from the date of entering the national phase 500 EUR" |
| OFF_ExaminationFee | per page above 20 (3-month tier) | 10 | 10 | same | MATCH | RO.txt "plus, for each page in excess of 20 10 EUR" |
| OFF_ExaminationFee | per claim above 5 (3-month tier) | 15 | 15 | same | MATCH | RO.txt "plus, for each claim in excess of 5 15 EUR" |
| Examination fee, later tier | paid after 3 months: 300 + 5/page > 20 + 10/claim > 5 | — | 300 / 5 / 10 | same | MISSING | RO.txt "paid after three months from the date of entering the national phase 300 EUR" |
| Publication fee | within 6 months of entry / on entry | — | 50 / 80 | same | MISSING | RO.txt "paid within six months of entry into the national phase 50 EUR ... paid on entry into the national phase 80 EUR" |
| Search fee | on request | — | 50 | same | MISSING | RO.txt "Search fee 50 EUR" |

Notes: as in AUDIT.md, the comment on OFF_PriorityFee about "3 months" belongs to the examination
fee; left as is. The chapter's summary lists the annual fee for the first three years (150 EUR),
which is due from the 3rd year and not at entry.

## RS: PCT-RS.ipf (RSD)

Sources: ZIS fee page "Таксе" (https://www.zis.gov.rs/prava/takse/, page modified 1 July 2026),
sources/RS-office.txt; Paragraf notice that harmonised dinar amounts of the republic administrative
fees apply from 1 July 2026 (Sl. glasnik RS 54/2026), sources/RS-office-3.txt; eGuide RS, valid as
from July 13, 2026 (sources/RS.txt). The official gazette page (pravno-informacioni-sistem.rs)
needs JavaScript and saved empty (sources/RS-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-01 (office table: harmonised amounts in force 1 July 2026).
Fees last changed: 1 July 2026 (annual harmonisation of dinar amounts).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | international application, up to 10 claims; natural persons −50% | 8210 | 10660 (5330) | 1 Jul 2026 | UPDATED (amount) | RS-office "За међународну пријаву патента ... 1) до десет патентних захтева 10660 5330" |
| OFF_ClaimFee | per claim above 10; natural persons −50% | 800 | 1040 (520) | 1 Jul 2026 | UPDATED (amount) | RS-office "2) за сваки следећи патентни захтев преко десет патентних захтева 1040 520" |
| OFF_SearchFee | no international search report; natural persons −50% | 16470 | 21390 (10695) | 1 Jul 2026 | UPDATED (amount) | RS-office "ако не постоји извештај о међународном претраживању ... 21390 10695" |
| OFF_Renewal3 | 3rd-year annual fee (from 24 months); natural persons −50% | 11510 | 14950 (7475) | 1 Jul 2026 | UPDATED (amount) | RS-office "1) трећу годину 14950 7475" |
| Examination fee | request for substantive examination; natural persons −50% | — | 10660 (5330) | 1 Jul 2026 | MISSING | RS-office "За захтев за суштинско испитивање ... 10660 5330"; RS.txt lists it under "National fees" |
| Late-entry surcharge | entry within 30 days after the time limit | — | +50% of the filing fee | not stated | MISSING | RS-office "увећаном за 50% ... у накнадном року од 30 дана"; RS.txt "plus 50% of the filing fee" |

Notes: the chapter (10,320 / 1,010 / 20,710 / 14,470 RSD) is newer as a document but its fee lines
are not in its July 2026 change list, so they are the previous year's harmonised level; the ZIS
table is preferred because its amounts are the ones in force from 1 July 2026. That the ZIS page
carries the July 2026 level is confirmed by its general items (Тарифни број 1 "430", Тарифни број 9
"740"), which equal the 1 July 2026 amounts in the Paragraf notice ("taksa za zahtev od 430 dinara"; "taksa za rešenje od 740 dinara"). The natural-person column in the ZIS table is exactly half,
consistent with the schedule's 50% reduction for Entity_Person.

## RU: PCT-RU.ipf (RUB)

Sources: Rospatent fee table, Appendix 1 to Regulation 941 (https://rospatent.gov.ru/ru/activities/dues/table,
published 3 October 2025), sources/RU-office-2.txt; Regulation 941 as amended to 23 September 2025,
in force 4 October 2025 (sources/RU-office-3.txt); Pravo.ru report that Decree 1278 of 18 September
2024 took effect on 5 October 2024 and abolished the 30% discount for electronic filing
(sources/RU-office-4.txt); eGuide RU, valid as from August 1, 2026 (sources/RU.txt). The English
Rospatent fee page (sources/RU-office.txt) dates from 2021 and gives no amounts.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2024-10-05 (the date from which the amounts now in the schedule apply, Decree 1278; the consolidated regulation is in force from 4 October 2025 without changing these amounts).
Fees last changed: 5 October 2024 (Decree 1278 of 18 September 2024).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 3300 | 4000 | 5 Oct 2024 | UPDATED (amount) | RU-office-2 item 1.1 "4000 + 1000 за каждый пункт формулы изобретения свыше 10"; RU.txt "Filing fee ... 4,000 RUB" |
| OFF_ClaimFee | per claim above 10 | 700 × 0.7 (online discount) | 1000, no online discount | 5 Oct 2024 | UPDATED (amount, condition) | RU.txt "claims fee for each claim in excess of ten 1,000 RUB"; RU-office-4 "отменена скидка в размере 30% на подачу заявления в электронной форме" |
| OFF_ExaminationFee | one independent claim | 12500 | 14000 | 5 Oct 2024 | UPDATED (amount) | RU-office-2 item 1.8 "14000 + 10000 за каждый независимый пункт формулы свыше 1"; RU.txt "for one independent claim 14,000 RUB" |
| OFF_ExaminationFee | per independent claim above 1 | 9200 | 10000 | 5 Oct 2024 | UPDATED (amount) | same; RU.txt "for each independent claim in excess of one 10,000 RUB" |
| OFF_ExaminationFee reduction | ISR by Rospatent | −50% | −50% | not stated | MATCH | RU.txt "50% where the international search report has been established by ... (Rospatent)"; RU-office-3 p. 19 "уменьшается на 50 процентов" |
| OFF_ExaminationFee reduction | ISR by another ISA | −10% | −10% | not stated | MATCH | RU.txt "10% where the international search report has been established by any of the other International Searching Authorities"; RU-office-3 p. 19 |
| Fee for claims not searched by the ISA | per unsearched claim | — | 1000 | 5 Oct 2024 | MISSING | RU-office-2 item 1.4 "+ 1000 за каждый пункт формулы международной заявки, по которой не был проведен поиск"; RU.txt "1,000 RUB" |
| Reduced fees (Appendix 2) | sole author, small business, accredited education or research body | — | e.g. filing 1000 + 250/claim or 1400 + 350/claim | 5 Oct 2024 | MISSING | RU-office-2 Appendix 2 item 1.1 "1000 + 250 за каждый пункт ... 1400 + 350 ..."; RU-office-3 pp. 14–16 |

Notes: the claim fee previously applied a 30% online-filing discount (0.7) that no longer exists;
the multiplier was removed and the COMPUTE comment updated. The baseline amounts were the pre-5
October 2024 ones, current at the baseline date. The Appendix 2 reductions are not mentioned in
the chapter, and whether foreign applicants can claim them was not established.

## SA: PCT-SA.ipf (SAR)

Sources: eGuide SA, valid as from February 1, 2026 (sources/SA.txt). The SAIP fee schedule was not
found as a savable page.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-02-01 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | non-individuals / individuals | 800/400 | 800/400 | not stated | MATCH | SA.txt "Filing fee 800 SAR (400 SAR)" |
| OFF_Renewal | 1st year (always) | 500/250 | 500/250 | not stated | MATCH | SA.txt "for the 1st year 500 SAR (250 SAR)" |
| OFF_Renewal | 2nd year (from 12 months) | 1000/500 | 1000/500 | not stated | MATCH | SA.txt "for the 2nd year 1,000 SAR (500 SAR)" |
| OFF_Renewal | 3rd year (from 24 months) | 1500/750 | 1500/750 | not stated | MATCH | SA.txt "for the 3rd year 1,500 SAR (750 SAR)" |

Notes: the examination fee is given only as "Please refer to the Office"; it is not established
and not counted.

## SC: PCT-SC.ipf (SCR)

Sources: eGuide SC, valid as from January 1, 2026 (sources/SC.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 3000 | 3000 | not stated | MATCH | SC.txt "National application fee 3,000 SCR" |

Notes: the chapter lists annual fees from the 2nd year (1,000 SCR per year for the 2nd to 4th
years); the schedule's comment says "annuities due as of 5 years" and models none. Not a
national-phase-entry fee, so not counted as missing, but the comment looks wrong against the
current chapter.

## SD: PCT-SD.ipf (SDG)

Sources: eGuide SD, valid as from March 20, 2025 (sources/SD.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-20 (chapter "Valid as from").
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 50 | 50 | not stated | MATCH | SD.txt "Filing fee 50 SDG" |

Notes: the annex also lists a publication fee (15 SDG) and an annual fee (10 SDG per year) without
saying when they fall due; not counted.

## Summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| PE | 4 | 4 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| PG | 4 | 3 | 0 | 0 | 0 | 1 | 2026-01-01 (chapter) | not stated |
| PH | 6 | 4 | 0 | 0 | 0 | 2 | 2026-01-01 (chapter) | not stated |
| PL | 4 | 4 | 0 | 0 | 0 | 0 | 2026-08-01 (chapter) | not stated |
| PT | 2 | 0 | 1 | 0 | 0 | 1 | 2026-07-01 (office table) | 1 Jul 2026 |
| QA | 3 | 2 | 1 | 0 | 0 | 0 | 2026-06-26 (chapter) | not stated (new amounts in chapter of 26 Jun 2026) |
| RO | 8 | 5 | 0 | 0 | 0 | 3 | 2026-01-01 (office table, per AUDIT.md) | EUR unchanged; lei indexed 1 Jan 2026 |
| RS | 6 | 0 | 4 | 0 | 0 | 2 | 2026-07-01 (office table) | 1 Jul 2026 |
| RU | 8 | 2 | 4 | 0 | 0 | 2 | 2024-10-05 (office regulation, Decree 1278) | 5 Oct 2024 |
| SA | 4 | 4 | 0 | 0 | 0 | 0 | 2026-02-01 (chapter) | not stated |
| SC | 1 | 1 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| SD | 1 | 1 | 0 | 0 | 0 | 0 | 2025-03-20 (chapter) | not stated |
| **Total** | **51** | **30** | **10** | **0** | **0** | **11** | | |
