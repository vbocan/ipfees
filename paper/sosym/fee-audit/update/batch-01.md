# Batch 01: AE AG AL AM AO AP AT AU AZ BA BB BG

Checked on 20 October 2026 against the WIPO PCT Applicant's Guide national chapters saved in
`sources/XX.txt` and, where an office table could be reached, the office's own pages
(`sources/XX-office*.txt`). Office fee tables were searched for every office; none could be found
online for AG, AL, AM, AO, BA and BB, and the Azerbaijani page found (`AZ-office.txt`, a 2022
notice) carries no invention filing fees. For AU, `../AUDIT.md` (batch 2) was used and confirmed
against the saved chapter and office pages. In the summary, "Rows" counts the modelled fee rows
(MATCH + UPDATED + REMOVED + UNVERIFIED); MISSING rows are counted separately.

## AE: PCT-AE.ipf (AED)

Sources: eGuide AE, valid as from 7 May 2026 (sources/AE.txt); MoET service page "Register
Patents", undated (sources/AE-office.txt); MoET "Pay Annual Fee", undated (sources/AE-office-2.txt);
MoET "Paying Annuity Fee", undated (sources/AE-office-3.txt); AIPPI note on Cabinet Decision
No 112 of 2023 (sources/AE-office-4.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-05-07 (the chapter's "Valid as from" date; the office pages carry no date).
Fees last changed: 15 January 2024 according to AE-office-4 ("Scheduled to take effect on 15 January 2024"); the eGuide annex does not reflect it.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | other / natural person | 2000/1000 | 2000/1000 | not stated | MATCH | AE "Filing fee 2,000 AED (1,000 AED)"; AE-office "AED 1,000 for individuals AED 2,000 for companies" |
| OFF_PublicationFee | other / natural person | 800/400 | 800/400 (eGuide) or none (office) | — | UNVERIFIED: eGuide and office disagree, office page undated | AE "Publication fee: 800 AED (400 AED)"; AE-office "Request 1st publication: No fees required" |
| OFF_Renewal | year 2, always | 800 | 800 (eGuide) or 200 per year for years 1–5 (office) | — | UNVERIFIED: eGuide and office disagree, office page undated | AE "for the 2nd year: 800 AED (400 AED)"; AE-office-3 "Annual Fees from 1st -5th year … AED 200 for( legal persons)" |
| OFF_Renewal | year 3, RenMonth ≥ 12 | 840 | 840 (eGuide) or 200 (office) | — | UNVERIFIED: as above | AE "for the 3rd year: 840 AED (420 AED)"; AE-office-3 as above |
| OFF_Renewal | year 4, RenMonth ≥ 24 | 880 | 880 (eGuide) or 200 (office) | — | UNVERIFIED: as above | AE "for the 4th year: 880 AED (440 AED)"; AE-office-3 as above |
| Search and first examination | on request | — | 7,000 | not stated | MISSING | AE-office "Request search and first examination: AED 7,000 for individuals AED 7,000 for companies" |

Notes: The eGuide chapter still cites Federal Law No. 31 of 2006 and its annex fees, while the
Ministry's service pages carry a different structure (no publication fee, annual fees of AED
200/100 per year for years 1–5 for legal persons / natural persons, SMEs and academia, a search and
first examination fee). AE-office-4 says Cabinet Decision No 112 of 2023 took effect on 15 January
2024 and introduced "annual maintenance fees (which used to be for zero dirhams)". The office pages
are probably the current law, but they are undated and not consistent with each other (the
register page distinguishes individuals and companies; the annuity page groups natural persons with
SMEs and academia; the late-payment charges differ between AE-office-2 and AE-office-3). AE-office-3
also says that for PCT national phase "it is calculated retroactively from the international filing
date", so three annual fees would be due at a 30-month entry. Nothing was changed; a decision on
which source to follow is needed. The baseline renewal ignores the natural-person rate the eGuide
gives (400/420/440). A web search reported Cabinet Resolution No. 136 of 2026 (in force 14 September
2026) exempting student inventors and People of Determination from filing and annual fees; no
official page was saved, so it is recorded only as a lead.

## AG: PCT-AG.ipf (XCD)

Sources: eGuide AG, valid as from 7 March 2025 (sources/AG.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-07 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 800 | 800 | not stated | MATCH | AG "Filing fee: 800 XCD" |
| OFF_ClaimFee | each claim > 20 | 10 | 10 | not stated | MATCH | AG "Fee for each claim in excess of 20: 10 XCD" |
| OFF_PublicationFee | always | 75 | 75 | not stated | MATCH | AG "Fee for publication of the application: 75 XCD" |
| OFF_GrantFee | optional | 1000+50 | 1000+50 | not stated | MATCH | AG "Fee for grant: 1,000 XCD"; "Fee for publication of grant of patent: 50 XCD" |
| OFF_Renewal | year 2, always | 200 | 200 | not stated | MATCH | AG "for the 2nd year: 200 XCD" |
| OFF_Renewal | year 3, RenMonth ≥ 12 | 300 | 300 | not stated | MATCH | AG "for the 3rd year: 300 XCD" |
| OFF_Renewal | year 4, RenMonth ≥ 24 | 400 | 400 | not stated | MATCH | AG "for the 4th year: 400 XCD" |

Notes: the grant-publication fee is "plus amount payable to publisher", which is not modelled.

## AL: PCT-AL.ipf (ALL)

Sources: eGuide AL, valid as from 12 December 2025 (sources/AL.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-12-12 (chapter's "Valid as from" date).
Fees last changed: not stated (the chapter now cites the new Law No. 53/2025 of 3 July 2025).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 7000 | 7000 | not stated | MATCH | AL "Filing fee 7,000 ALL" |
| OFF_GrantFee | optional | 6000 | 6000 | not stated | MATCH | AL "Fee for grant: 6,000 ALL" |
| OFF_Renewal | always | 0 ("no annuities before grant") | annual fees on applications, from the 2nd or 3rd year | — | UNVERIFIED: rule changed, but the year is ambiguous and the fee cannot be modelled without a new LET | AL.07 "The maintenance of the application is subject to the payment of annual fees … for the third year"; annex "for the 2nd year: 5,000 ALL", "for the 3rd year: 6,000 ALL" |

Notes: under the new law annual fees are due while the application is pending, so the baseline
zero no longer holds; AL.07 names the third year while the annex starts at the second. The fee was
left at 0.

## AM: PCT-AM.ipf (AMD)

Sources: eGuide AM, valid as from 1 January 2026 (sources/AM.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always; −75 % person/<25 employees, −50 % 25–100 | 20000 | 20000 | not stated | MATCH | AM "Filing fee … 20,000 AMD"; "reduced by 75% … and by 50% for legal entities with 25 to 100 employees" |
| OFF_ClaimFee | each claim > 5 | 5000 | 5000 | not stated | MATCH | AM "Additional fee for each claim in excess of five … 5,000 AMD" |
| OFF_PriorityFee | per priority | 10000 | 10000 | not stated | MATCH | AM "Fee for priority claims, per priority 10,000 AMD" |
| OFF_GrantFee | optional | 15000 | 15000 | not stated | MATCH | AM "Fee for grant 15,000 AMD" |
| OFF_PublicationFee | each sheet > 25 | 2500 | 2500 | not stated | MATCH | AM "Publication fee for each sheet of description of an invention in excess of 25 2,500 AMD" |
| OFF_Renewal | always | 0 ("no annuities before grant") | 20,000 per year for years 2 and 3, listed among the national fees | 1 Jan 2026 (added to the summary) | UNVERIFIED: the chapter now lists the annual fees among the national fees but gives no time for payment | AM summary "Annual fees for the 2nd and the 3rd year, per year 20,000 AMD" |

Notes: the annual-fee line is new in the 1 January 2026 summary (listed under the changes). If
it is payable at entry, the renewal would be 2 × 20,000 with the same reductions. The publication
fee counts sheets of the description only; the schedule uses SheetCount.

## AO: PCT-AO.ipf (AOA)

Sources: eGuide AO, valid as from 4 December 2025 (sources/AO.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-12-04 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | up to 15 claims | 27722 | 27722 | not stated | MATCH | AO "Filing fee and publication fee up to 15 claims 27,722 AOA" |
| OFF_BasicNationalFee | each claim > 15 | 792 | 792 | not stated | MATCH | AO "For each claim over 15 plus 792 AOA" |
| OFF_ExaminationFee | optional | 27280 | 27280 | not stated | MATCH | AO "Examination fee 27,280 AOA" |
| OFF_Renewal | 1st anniversary, always | 3784 | 3784 | not stated | MATCH | AO "for the 1st anniversary 3,784 AOA" |
| OFF_Renewal | 2nd anniversary, RenMonth ≥ 12 | 4050 | 4050 | not stated | MATCH | AO "for the 2nd anniversary 4,050 AOA" |
| OFF_Renewal | 3rd anniversary, RenMonth ≥ 24 | 4400 | 4400 | not stated | MATCH | AO "for the 3rd anniversary 4,400 AOA" |

## AP: PCT-AP.ipf (USD)

Sources: eGuide AP, valid as from 27 February 2026 (sources/AP.txt); Harare Protocol 2025 edition,
Schedule I: Fees (sources/AP-office.txt); ARIPO notice on the amendments (sources/AP-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-01 (effective date of the office's current fee schedule, AP-office-2 "Patent fees Effective 1st March 2025").
Fees last changed: 1 March 2025.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (online) | 232 | 232 | not stated | MATCH | AP-office "Electronic filing (including 20% reduction) 232"; AP "online 232 USD on paper 290 USD" |
| OFF_DesignationFee | per designated State | 85 | 100 | 1 Mar 2025 | UPDATED (amount) | AP-office "Designation fee per designated State 100" |
| OFF_PublicationFee | always | 350 | 500 | 1 Mar 2025 | UPDATED (amount) | AP-office "Publication or republication 500" |
| OFF_PublicationFee | each page > 30 | 20 | 20 (pages 31–100), 30 (from 101) | 1 Mar 2025 | UPDATED (threshold) | AP-office "additional page from 31 to 100 pages 20"; "from 101 and each subsequent page thereafter 30" |
| OFF_PublicationFee | each claim > 10 | 50 | 100 (claims 11–50), 200 (from 51) | 1 Mar 2025 | UPDATED (amount, threshold) | AP-office "additional claim from 11 to 50 claims 100"; "from 51 … 200" |
| OFF_SearchFee | no ISR | 300 | 300 | not stated | MATCH | AP-office "Prior art search 300" |
| OFF_ExaminationFee | always | 300 | 1000 | 1 Mar 2025 | UPDATED (amount) | AP-office "Request for substantive examination 1000" |
| OFF_GrantFee | optional | 350 | 500 | 1 Mar 2025 | UPDATED (amount) | AP-office "Grant 500" |
| OFF_Renewal | 1st anniversary, always | 50 | 50 | not stated | MATCH | AP-office "1st anniversary 50" |
| OFF_Renewal | 2nd anniversary, RenMonth ≥ 12 | 70 | 70 | not stated | MATCH | AP-office "2nd anniversary 70" |
| OFF_Renewal | 3rd anniversary, RenMonth ≥ 24 | 90 | 90 | not stated | MATCH | AP-office "3rd anniversary 90" |

Notes: the tiered surcharges are written as two yields each (20 per page over 30 plus 10 per page
over 100; 100 per claim over 10 plus 100 per claim over 50), which gives the official 20/30 and
100/200 rates. The ARIPO notice says the new fees "apply to all existing and new applications".
The annual fees are "per designated State" (AP-office) but the schedule charges them once; not
changed. The eGuide exempts search and examination fees only for applications filed on or before
1 January 2017 when an ISR exists, so the baseline condition `ISR_AVAILABLE EQ FALSE` on the search
fee may be too narrow; not changed.

## AT: PCT-AT.ipf (EUR)

Sources: eGuide AT, valid as from 1 April 2026 (sources/AT.txt); Austrian Patent Office page
"Anmelde- und Veröffentlichungsgebühren", undated, retrieved 20 October 2026 (sources/AT-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-04-01 (chapter's "Valid as from" date; the office page is undated).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | entry fee | 52 | 52 | not stated | MATCH | AT "Fee for entry into the national phase 52 EUR"; AT-office "Einleitung der nationalen Phase pro Schutzrecht EUR 52" |
| OFF_BasicNationalFee | document fee | 50 | 74 | not stated | UPDATED (amount) | AT "Document fee ( Schriftengebühr ) 74 EUR"; AT-office "inkl. Schriftengebühr EUR74" |
| OFF_SearchExaminationFee | including 10 claims | 292 | 292 − 20 (electronic) | not stated | UPDATED (amount: electronic rate) | AT "Search and examination fee, including 10 claims 292 EUR"; AT-office "Ermäßigung bei elektronischer Einbringung des Antrags auf Einleitung EUR -20" |
| OFF_SearchExaminationFee | each 10 claims > 10 | 104 | 104 | not stated | MATCH | AT "for each group of up to 10 claims 104 EUR" |
| OFF_PublicationFee | optional | 208 | 208 | not stated | MATCH | AT "Publication fee (for grant) … 208 EUR" |
| OFF_PublicationFee | each 15 pages > 15 | 135 | 135 | not stated | MATCH | AT "additional fee of 135 EUR is payable for each 15 pages" |
| OFF_Renewal | at entry | 0 | 0 (first annual fee is for the 6th year) | — | MATCH | AT "Annual fees: — for the 6th year 104 EUR" |
| Entry fee exemption | RO = Austrian Patent Office | — | entry fee not payable | not stated | MISSING | AT "No fee for entry into the national phase is payable if the international application was filed with the Austrian Patent Office as receiving Office" |

Notes: the office page gives the search and examination fee at entry as EUR 366 "inkl.
Schriftengebühr EUR74" with a EUR 20 reduction for electronic filing of the entry request; the
eGuide gives the paper amount (292 + 74). Following the corpus rule (electronic rate), the
reduction is written as a separate yield, `YIELD -20`, in OFF_SearchExaminationFee.

## AU: PCT-AU.ipf (AUD)

Sources: eGuide AU, valid as from 20 August 2026 (sources/AU.txt); IP Australia "Timeframes and
fees", retrieved 20 October 2026 (sources/AU-office.txt); IP Australia summary table of the fee
changes of 1 October 2024 (sources/AU-office-2.txt); ../AUDIT.md batch 2.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2024-10-01 (effective date of the office's current fees, AU-office-2 "fee changes that IP Australia will implement on 1 October 2024").
Fees last changed: 1 October 2024.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (online) | 370 | 400 | 1 Oct 2024 | UPDATED (amount) | AU "national phase entry of a PCT application … 400 AUD (600 AUD)"; AU-office-2 "Entry of PCT application into national phase … $370 $400" |
| OFF_ExaminationFee | IPER by IP Australia | 300 | 350 | 1 Oct 2024 | UPDATED (amount) | AU "(a) … international preliminary examination report … 350 AUD"; AU-office-2 "$300 $350" |
| OFF_ExaminationFee | other cases | 490 | 550 | 1 Oct 2024 | UPDATED (amount) | AU "(b) all other applications 550 AUD"; AU-office-2 "in other cases $490 $550" |
| Excess claims at first report | 21st–30th; > 30 | — | 125 each; 250 each | 1 Oct 2024 | MISSING | AU "amount for each of the 21st to the 30th claims 125 AUD"; "each claim in excess of 30 250 AUD" |
| Acceptance | minimum | — | 300 | 1 Oct 2024 | MISSING | AU "Minimum payable in all cases 300 AUD" |
| Continuation fees | from 4th anniversary | — | 300, 315, 345 … | 1 Oct 2024 (6th onward) | MISSING | AU "on the 4th anniversary 300 AUD (350 AUD)" |

Notes: agrees with ../AUDIT.md batch 2. The `IPRP EQ IPRP_AU` condition matches the official
wording.

## AZ: PCT-AZ.ipf (AZN)

Sources: eGuide AZ, valid as from 2 July 2026 (sources/AZ.txt); IP Agency notice on the fee changes
of Cabinet decision No. 198 of 19 May 2022 (sources/AZ-office.txt; no invention filing fees in it).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-07-02 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 20 | 20 | not stated | MATCH | AZ "Filing fee (including examination) … 20 AZN" |
| OFF_ClaimFee | each independent claim > 1 | 7 | 7 | not stated | MATCH | AZ "Additional fee for each independent claim in excess of one 7 AZN" |
| OFF_ClaimFee | each claim > 10 | 7 | 7 per dependent claim > 10 | not stated | MATCH (amount; basis differs, see notes) | AZ "Additional fee for each dependent claim in excess of ten 7 AZN" |
| OFF_Renewal | 3rd year, RenMonth ≥ 24; other / natural persons | 50/10 | 50/10 | not stated | MATCH | AZ "Annual fee for the 3rd year 50 AZN — where all applicants are natural persons 10 AZN" |

Notes: the office charges for dependent claims over ten; the schedule counts all claims over ten
(`ClaimCount`), although `DependentClaimCount` exists. Not changed.

## BA: PCT-BA.ipf (BAM)

Sources: eGuide BA, valid as from 8 January 2026 (sources/BA.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-08 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 260 | 260 | not stated | MATCH | BA "Filing and publication fee 260 BAM" |
| OFF_PublicationFee | optional | 60 | 60 | not stated | MATCH | BA "Publication fee for granted patent 60 BAM" |
| OFF_PublicationFee | each claim > 10 | 11 | 11 | not stated | MATCH | BA "for each claim over 10 plus 11 BAM" |
| OFF_PublicationFee | each page > 30 | 3 | 3 | not stated | MATCH | BA "for each page of the application over 30 plus 3 BAM" |
| OFF_Renewal | at entry | 0 | 0 (after grant) | — | MATCH | BA.04 "After a patent has been granted, maintenance fees must be paid" |
| Applicant-inventor reduction | applicant is inventor | — | filing fee −50 % | not stated | MISSING | BA "The filing fee is reduced by 50% where the applicant is also the inventor" |

Notes: in the chapter the per-claim and per-page surcharges belong to the filing and publication
fee due at entry; the schedule places them in the optional OFF_PublicationFee (grant
publication), so they drop out when that fee is deselected. Amounts unchanged; structure not
changed.

## BB: PCT-BB.ipf (BBD)

Sources: eGuide BB, valid as from 5 June 2026 (sources/BB.txt); no office table found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-06-05 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 300 | 300 | not stated | MATCH | BB "Filing fee … 300 BBD" |
| OFF_Renewal | before 2nd anniversary, RenMonth ≥ 12 | 200 | 200 | not stated | MATCH | BB "before the 2nd anniversary of filing 200 BBD" |
| OFF_Renewal | before 3rd anniversary, RenMonth ≥ 24 | 300 | 300 | not stated | MATCH | BB "before the 3rd anniversary of filing 300 BBD" |

Notes: the grant fee (300 BBD, "must be paid before the grant") is not modelled; it is not a
national-phase-entry fee.

## BG: PCT-BG.ipf (BGN)

Sources: eGuide BG, valid as from 26 June 2026 (sources/BG.txt); WIPO Lex record of the Tariff
(sources/BG-office.txt, "Entry into force: January 1, 2026"); Tariff of Fees, Council of
Ministers Decree No. 13 of 27 February 2025, English text (sources/BG-office-2.txt).
Version line: unchanged, '2024.1' EFFECTIVE 2024-01-01 (no amount in the schedule's currency is in force; see notes).
Fees last changed: 1 January 2026 (euro tariff).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | filing + publication | 40+70 BGN | 20.45 + 35.79 EUR | 1 Jan 2026 | UPDATED (amount, currency; lead decision, README.md) | BG-office-2 "1. Filing 20,45"; "9. Publication of an application in the Official Bulletin 35,79" |
| OFF_ExaminationFee | always | 180 BGN | 92.03 EUR | 1 Jan 2026 | UPDATED (amount, currency; lead decision, README.md) | BG-office-2 "7. Search and examination of an application for one invention 92,03" |
| OFF_PriorityFee | per priority | 20 BGN | 10.23 EUR | 1 Jan 2026 | UPDATED (amount, currency; lead decision, README.md) | BG-office-2 "Claiming priority - for each priority 10,23 each" |
| Claims fee | each claim > 10 | — | 10.23 EUR (Tariff) / 0 EUR (eGuide) | 1 Jan 2026 | MISSING | BG-office-2 "for each claim following after the 10th 10,23"; BG "Claims fee for each claim in excess of 10 0 EUR" |
| Examination of formal requirements | always | — | 20.45 EUR | 1 Jan 2026 | MISSING | BG-office-2 "4. Examination of formal requirements 20,45" |
| Preliminary examination and admissibility | one invention / group | — | 76.69 / 92.03 EUR | 1 Jan 2026 | MISSING | BG-office-2 "for one invention 76,69"; "for a group of inventions 92,03" |
| 50 % reduction | inventors, micro and small enterprises, schools, universities, research organisations | — | filing and examination fees −50 % | 1 Jan 2026 | MISSING | BG-office-2 "shall be paid at 50 per cent … where the applicants are inventors, micro and small enterprises" |
| 50 % reduction | licensing-readiness declaration at entry | — | patent fees −50 % | not stated | MISSING | BG.03 "reduced by 50% in case of a declaration of readiness for licensing" |

Notes: Bulgaria now charges in euro (BG "All payments of fees must be effected … in Euro"; the
Tariff is "In force from the date specified in the Decision of the Council of the European Union
on the adoption of the euro"). Each euro amount equals the baseline lev amount divided by
1.95583 (40 → 20.45, 70 → 35.79, 180 → 92.03, 20 → 10.23); that rate is not in a saved source.
Per the protocol the schedule was not converted, and its version line and header were left
unchanged pending a decision on the currency. BG.07 lists the fees due at entry: filing, formal
requirements examination, preliminary examination and admissibility, search and examination,
claims and priority fees. The eGuide's claims fee of 0 EUR conflicts with the Tariff's 10.23 EUR.
The Tariff also lists "Handling international correspondence relating to a patent application
under the PCT 40,90"; whether it applies at national-phase entry is unclear, so it is not listed
as MISSING.

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| AE | 5 | 1 | 0 | 0 | 4 | 1 | 2026-05-07 (chapter) | 15 Jan 2024 (Cabinet Decision 112/2023, per AIPPI) |
| AG | 7 | 7 | 0 | 0 | 0 | 0 | 2025-03-07 (chapter) | not stated |
| AL | 3 | 2 | 0 | 0 | 1 | 0 | 2025-12-12 (chapter) | not stated |
| AM | 6 | 5 | 0 | 0 | 1 | 0 | 2026-01-01 (chapter) | not stated |
| AO | 6 | 6 | 0 | 0 | 0 | 0 | 2025-12-04 (chapter) | not stated |
| AP | 11 | 5 | 6 | 0 | 0 | 0 | 2025-03-01 (office) | 1 Mar 2025 |
| AT | 7 | 5 | 2 | 0 | 0 | 1 | 2026-04-01 (chapter) | not stated |
| AU | 3 | 0 | 3 | 0 | 0 | 3 | 2024-10-01 (office) | 1 Oct 2024 |
| AZ | 4 | 4 | 0 | 0 | 0 | 0 | 2026-07-02 (chapter) | not stated |
| BA | 5 | 5 | 0 | 0 | 0 | 1 | 2026-01-08 (chapter) | not stated |
| BB | 3 | 3 | 0 | 0 | 0 | 0 | 2026-06-05 (chapter) | not stated |
| BG | 3 | 0 | 0 | 0 | 3 | 5 | unchanged, 2024-01-01 (currency pending) | 1 Jan 2026 (euro tariff) |
| **Total** | **63** | **43** | **11** | **0** | **9** | **11** | | |
