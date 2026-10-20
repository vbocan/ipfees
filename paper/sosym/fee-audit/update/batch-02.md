# Batch 02: BH BN BR BW BY BZ CA CH CN CO CR CU

Checked on 20 October 2026 against the WIPO PCT Applicant's Guide national chapters saved in
`sources/XX.txt` and, where they could be reached, the offices' own tables (`sources/XX-office*.txt`).
For BR, CA, CH and CN the audit of 19 October 2026 (`../AUDIT.md`) was confirmed against the saved
chapter and office pages. Baseline is IPFees `beab6fd` (every file `VERSION '2024.1' EFFECTIVE
2024-01-01`). Amounts written "a/b" are standard/reduced unless stated otherwise.

## BH: PCT-BH.ipf (BHD)

Sources: eGuide BH, valid as from 1 October 2026 (sources/BH.txt); WIPO Lex record of Ministerial Decision No. 102 of 2018 as amended by Ministerial Order No. 67 of 2023 (sources/BH-office.txt); Mondaq note on the 2023 individual fees, secondary, used for dates only (sources/BH-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2023-09-22 (entry into force of the current consolidated fee schedule, Order No. 67 of 2023, as stated by WIPO Lex).
Fees last changed: 22 September 2023 (individual rates).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | other / natural person | 540/270 | 540/40 | 22 Sep 2023 (before baseline) | UPDATED (amount) | BH "Filing fee ... 540 BHD (40 BHD)"; "The amount in parentheses is payable in case of filing by an individual"; BH-office "amendments to the fees ... for individuals (in force from 22/09/2023)" |
| OFF_PublicationFee | other / natural person | 320/160 | 320/50 | 22 Sep 2023 (before baseline) | UPDATED (amount) | BH "Publication fee 320 BHD (50 BHD)"; BH-office-2 "Publication and grant fees 160 BHD ... 50 BHD" |
| OFF_Renewal | 2nd annual fee, from 12 months | 200 for all | 200/100 | not stated | UPDATED (condition) | BH "2nd annual fee 200 BHD (100 BHD)" |
| Examination fee, including search | — | — | "According to actual cost" (250 for individuals per secondary source) | 22 Sep 2023 | MISSING | BH "Examination fee, including search According to actual cost" |

Notes: the individual rates were cut by Ministerial Order No. 67 of 2023, so the baseline person
rates (270, 160) were already out of date on 1 January 2024. The renewal fee now distinguishes
natural persons, as the office does; the condition `RenMonth GTE 12` is kept as it was. The BHD
amounts of the examination fee for legal entities are not published in the chapter.

## BN: PCT-BN.ipf (BND)

Sources: eGuide BN, valid as from 7 March 2025 (sources/BN.txt). BruIPO's own fee table could not be found (searches returned only the Patents Rules 2012 on WIPO Lex, without amounts).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-07 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 160 | 160 | not stated | MATCH | BN "Filing fee 160 BND" |
| OFF_ExaminationFee | optional (request for examination) | 1100 | 1100 | not stated | MATCH | BN "Request for examination 1,100 BND" |
| Request for search and examination | alternative to the examination request when no usable search exists | — | 2600 | not stated | MISSING | BN "Request for search and examination 2,600 BND" |

Notes: annual fees start in the 5th year (BN.08), so none is due at entry, as the schedule says.

## BR: PCT-BR.ipf (BRL)

Sources: eGuide BR, valid as from 1 October 2026 (sources/BR.txt); INPI "Custos e pagamento" page (sources/BR-office.txt); INPI Tabela de Retribuições, "Portaria GM/MDIC nº 110/2025, Portaria INPI/PR nº 10/2025 e apostila 31/10/2025" (sources/BR-office-2.txt); AUDIT.md batch 2.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-08-07 (effective date of the office's current table).
Fees last changed: 7 August 2025.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | electronic | 175 | 260 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "200 ... Por meio eletrônico 260,00"; BR "260 BRL" |
| OFF_ExaminationFee | up to 10 claims | 590 | 870 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "Até 10 (dez) reivindicações 870,00"; BR "no more than 10 claims 870 BRL" |
| OFF_ClaimFee | each of claims 11–15 | 100 | 140 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "R$ 140,00 por reivindicação da 11ª a 15ª" |
| OFF_ClaimFee | each of claims 16–30 | 200 | 290 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "R$ 290,00 por reivindicação da 16ª a 30ª" |
| OFF_ClaimFee | each claim from the 31st | 500 | 740 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "R$ 740,00 por reivindicação da 31ª em diante" |
| OFF_Renewal | first annuity, from 24 months | 780 | 400 | 7 Aug 2025 | UPDATED (amount) | BR-office-2 "220 - no prazo ordinário 400,00"; BR "First annual fee ... 24 months ... 400 BRL" |
| Examination via PCT (code 284) | INPI was ISA/IPEA | — | 570 + same claim surcharges | 7 Aug 2025 | MISSING | BR-office-2 "via PCT ... Até 10 (dez) reivindicações 570,00" |
| 50 % discount | natural persons, ME/EPP, ICTs, non-profits, public bodies | — | 50 % | 7 Aug 2025 | MISSING | BR-office-2 "Valor com desconto*"; BR "reduced by 50% under certain conditions" |

Notes: paper entry costs 380 (not modelled; the corpus uses electronic rates). The schedule has
no ISA_BR choice, so code 284 cannot be expressed without a new input. The claim tiers (10, 15,
30) are unchanged. The renewal comment now names the fee (code 220) rather than "year 3".

## BW: PCT-BW.ipf (BWP)

Sources: eGuide BW, valid as from 1 August 2026 (sources/BW.txt). CIPA's own fee schedule could not be found online (only third-party summaries and the 2010 regulations on WIPO Lex).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-08-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | standard / individual or ≤100 employees | 300/150 | 300/150 | not stated | MATCH | BW "Application fee ... 150 BWP (300 BWP)"; "Fees are reduced by 50% if the applicant is an individual or a small entity whose number of employees does not exceed 100" |
| OFF_GrantFee | optional | 300/150 | 300/150 | not stated | MATCH | BW "Grant and publication fee 300 BWP" |
| OFF_Renewal | 1st year | 60/30 | 60/30 | not stated | MATCH | BW "for the 1st year 60 BWP" |
| OFF_Renewal | 2nd year, from 12 months | 240/120 | 240/120 | not stated | MATCH | BW "for the 2nd year 240 BWP" |
| OFF_Renewal | 3rd year, from 24 months | 300/150 | 300/150 | not stated | MATCH | BW "for the 3rd year 300 BWP" |

Notes: the reduction is applied to Entity_Person, Less25 and Less100; Entity_Small and
Entity_Micro (which would usually also have ≤100 employees) pay the full rate in the schedule.
Unchanged, as it is a mapping question rather than an amount.

## BY: PCT-BY.ipf (BYN)

Sources: eGuide BY, valid as from 15 May 2026 (sources/BY.txt); NCIP extract of Tax Code annex 23, rates for inventions in base values (sources/BY-office-2.txt); Tax Code art. 298, payment rules (sources/BY-office.txt); Tax Code reliefs, art. 296–297 (sources/BY-office-4.txt); Pravo.by on the base value from 1 January 2026 (sources/BY-office-3.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (date from which the base value of 45 BYN, and so the BYN amounts, apply).
Fees last changed: 1 January 2026 (base value 42 → 45 BYN; the rates in base values are unchanged).

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 90.65 | 110.25 | 1 Jan 2026 | UPDATED (amount) | BY "Filing fee ... 110.25 BYN"; BY-office-2 "одно изобретение 2,45"; BY-office-3 "С 1 января 2026 г. размер базовой величины составит 45 рублей" |
| OFF_ClaimFee | each independent claim over one | 38.85 | 47.25 | 1 Jan 2026 | UPDATED (amount) | BY "Claim fee for each independent claim in excess of one 47.25 BYN"; BY-office-2 "дополнительно 1,05" |
| OFF_ExaminationFee | no ISR / ISR available (50 %) | 440.30/220.15 | 535.50/267.75 | 1 Jan 2026 | UPDATED (amount) | BY "Examination fee 535.50 BYN"; "reduced by 50% where an international search report ... has been established"; BY-office-2 "одного изобретения 11,9" |
| OFF_ExaminationFee | each independent claim over one | 259 | 315 | 1 Jan 2026 | UPDATED (amount) | BY "Claim examination fee for each independent claim in excess of one 315 BYN"; BY-office-2 "дополнительно 7 за каждый независимый пункт" |
| OFF_GrantFee | optional | 181.30 | 220.50 | 1 Jan 2026 | UPDATED (amount) | BY "Fee for grant 220.50 BYN"; BY-office-2 "выдача патента на изобретение 4,9" |
| Applicant-author reduction | authors pay 25 % | — | 25 % of most fees | not stated | MISSING | BY-office-4 "Плательщики-авторы уплачивают 25 процентов от установленного размера" |
| E-filing reduction | filing fee (item 1.1) filed electronically | — | 85 % | not stated | MISSING | BY-office-4 "уплачиваются в размере 85 процентов ... подачи заявки через систему электронной подачи" |

Notes: the baseline amounts are the same rates at the 2023 base value of 37 BYN (90.65 = 2.45 × 37),
so they were out of date from 1 January 2024 (base value 40), 2025 (42) and 2026 (45). Non-residents
pay the BYN amount in USD, EUR, CHF or RUB at the National Bank rate (eGuide BY, Tax Code art. 298);
the schedule keeps BYN. Doubt: Tax Code art. 296(4) grants the 50 % ISR reduction "в отношении
изобретений, заявленных в независимых пунктах формулы, по которым подготовлены ... отчет о
поиске", which may extend it to the per-independent-claim examination fee; the schedule's comment
"no discount seems to apply here" was left as it was. Whether the 85 % e-filing rate reaches the
national-phase fee (item 1.17, "в размере патентных пошлин за подачу") is not stated. The
`VERIFY MONOTONIC` on OFF_ClaimFee is declared against ClaimCount while the fee depends on
IndependentClaimCount (unchanged).

## BZ: PCT-BZ.ipf (BZD)

Sources: eGuide BZ, valid as from 1 January 2026 (sources/BZ.txt); Patents (Amendment) Regulations 2022, S.I. No. 26 of 2022, new Schedule I, in force 1 March 2022 (sources/BZ-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2022-03-01 (commencement of the office's current fee schedule).
Fees last changed: 1 March 2022.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 300 | 450 | 1 Mar 2022 (before baseline) | UPDATED (amount) | BZ-office "On application for the grant of a Patent $450.00 (including 1st annuity fee)"; BZ "Filing fee (including 1st annual fee) ... 450 BZD" |
| OFF_GrantFee | optional | 300 | 550 | 1 Mar 2022 (before baseline) | UPDATED (amount) | BZ-office "For publication and grant of a patent $550.00"; BZ "Fee for grant 550 BZD" |
| OFF_Renewal | 1st annuity | 100 | 100 | 1 Mar 2022 | REMOVED (included in the 450 filing fee; lead decision, README.md) | BZ-office "before the 1st anniversary of filing $100.00" |
| OFF_Renewal | 2nd annuity, from 12 months | 200 | 200 | 1 Mar 2022 | MATCH | BZ-office "before the 2nd anniversary of filing $200.00" |
| OFF_Renewal | 3rd annuity, from 24 months | 300 | 300 | 1 Mar 2022 | MATCH | BZ-office "before the 3rd anniversary of filing $300.00" |

Notes: the filing fee is stated as "including 1st annuity fee", while the chapter's summary also
lists "Annual fees for the first three years 600 BZD" (100 + 200 + 300) as payable at entry. The
schedule still charges the 1st annuity (100) unconditionally next to the 450 filing fee, so it may
count the first annuity twice; left unchanged because the two official statements disagree.

## CA: PCT-CA.ipf (CAD)

Sources: eGuide CA, valid as from 31 August 2026 (sources/CA.txt); CIPO "2026 list of fees for patents" and "2027 list of fees for patents" (sources/CA-office.txt); AUDIT.md batch 2.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (effective date of the office's current fees: "On January 1, 2026, certain fees ... increased by 2.7%").
Fees last changed: 1 January 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | standard / small entity | 407.18/203.59 | 595.06/241.24 | 1 Jan 2026 | UPDATED (amount) | CA-office "Fee for entering national phase in Canada 241.24 595.06"; CA "Basic national fee 595.06 CAD (241.24 CAD)" |
| OFF_ExaminationFee | ISA was CIPO | 203.59/100 | 297.00/117.94 | 1 Jan 2026 | UPDATED (amount) | CA-office "international search by the Commissioner ... 117.94 297.00" |
| OFF_ExaminationFee | any other ISA | 814.37/407.18 | 1190.13/482.48 | 1 Jan 2026 | UPDATED (amount) | CA-office "In any other case ... 482.48 1,190.13" |
| OFF_Renewal | 2nd, 3rd and 4th anniversary | 100/50 each | 134.02/60.26 each | 1 Jan 2026 | UPDATED (amount) | CA-office "second, third and fourth anniversaries ... 60.26 134.02" |
| Excess claims | each claim over 20, with the examination request | — | 117.94/58.97 | 1 Jan 2026 | MISSING | CA-office "Fee for each claim in excess of 20 ... 58.97 117.94" |
| Late national entry | reinstatement plus late fee | — | 297.00 + 150.00 | 1 Jan 2026 | MISSING | CA-office "reinstatement of rights ... 297.00"; "Additional fee for late payment under subsection 154(4) ... 150.00" |

Notes: announced change from 1 January 2027 (not applied): basic national fee 605.18/245.34;
examination 302.05/119.94 with CIPO as ISA, 1,210.36/490.68 otherwise; excess claims 119.94/59.97;
maintenance 136.30/61.28 (CA-office "On January 1, 2027, certain fees ... will increase by 1.7%").
Only Entity_Small pays the small-entity rate; CIPO's small entity (fewer than 100 employees,
universities) would also cover Less25, Less100 and Micro. Doubt on the renewal conditions (kept as
at baseline): the 2nd-anniversary fee is charged always, the 3rd from 12 months and the 4th from 24
months after the application date, whereas CIPO ties them to the 2nd, 3rd and 4th anniversaries.

## CH: PCT-CH.ipf (CHF)

Sources: eGuide CH, valid as from 4 April 2024 (sources/CH.txt); IPI patent fees page, "Status per 1 July 2019" (sources/CH-office.txt); IPI overview of the fees from 1 January 2027, dated 20 May 2026 (sources/CH-office-2.txt); AUDIT.md batch 3.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2019-07-01 (date of the office's current table).
Fees last changed: 1 July 2019.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 200 | 200 | 1 Jul 2019 | MATCH | CH-office "Application fee CHF 200"; CH "Filing fee 200 CHF" |
| OFF_ClaimFee | each claim from the 11th | 50 | 50 | 1 Jul 2019 | MATCH | CH-office "Claims fee for each additional claim from the 11th patent claim CHF 50" |
| OFF_ExaminationFee | optional | 500 | 500 | 1 Jul 2019 | MATCH | CH-office "Examination fee CHF 500"; CH "Examination fee 500 CHF" |
| Search | optional | — | 500 | 1 Jul 2019 | MISSING | CH-office "Swiss patent application search CHF 500" |

Notes: announced change from 1 January 2027 (not applied): claims fee from the 16th claim, search
mandatory at 500, examination 400, full substantive examination 300 more (CH-office-2 "Fee in CHF
from 1 January 2027"; transitional provisions not read). Annual fees start in the 4th year (CH.09),
so none is due at entry.

## CN: PCT-CN.ipf (CNY)

Sources: eGuide CN, valid as from 1 October 2026 (sources/CN.txt); CNIPA Announcement No. 594, published 6 August 2024 (sources/CN-office.txt; saved with `curl --compressed`, since the plain download came back gzip-encoded); AUDIT.md batch 1.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-10-01 (chapter's "Valid as from"; the amounts date from 2017 and Announcement 594 applies "自上述通知发布之日起", without a stated date).
Fees last changed: amounts 1 July 2017 (AUDIT.md); PCT exemptions rewritten by Announcement 594, 6 August 2024.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | not waived | 900 | 900 | 1 Jul 2017 | MATCH | CN "Filing fee ... 900 CNY" |
| OFF_ClaimFee | each claim over 10 | 150 | 150 | 1 Jul 2017 | MATCH | CN "for each claim in excess of 10 ... 150 CNY" |
| OFF_SheetFee | sheets 31–300 / over 300 | 50/100 | 50/100 | 1 Jul 2017 | MATCH | CN "in excess of 30 sheets 50 CNY"; "in excess of 300 sheets 100 CNY" |
| Filing, claim and sheet fee waiver | baseline: CNIPA was RO; now: CNIPA was RO and ISA | 0 if RO | 0 if RO and ISA | 6 Aug 2024 | UPDATED (condition) | CN-office "由中国国家知识产权局作为受理局受理并进行国际检索的国际专利申请 ... 免缴申请费及申请附加费"; CN "exempted when ... (CNIPA) acts as the Receiving Office and the International Searching Authority" |
| OFF_PublicationFee | always | 50 | 50 | 1 Jul 2017 | MATCH | CN "Application publication fee ... 50 CNY" |
| OFF_PriorityFee | per priority | 80 | 80 | 1 Jul 2017 | MATCH | CN "per priority 80 CNY" |
| OFF_ExaminationFee | standard | 2500 | 2500 | 1 Jul 2017 | MATCH | CN "Examination fee ... 2,500 CNY" |
| OFF_ExaminationFee | exempt if CNIPA drew up the ISR or the IPRP | 0 | 0 | 6 Aug 2024 | MATCH | CN-office "由中国国家知识产权局作出国际检索报告或专利性国际初步报告的PCT申请 ... 免缴实质审查费" |
| OFF_ExaminationFee | 20 % reduction, ISA EP/JP/SE | 2000 | no longer official | 6 Aug 2024 | REMOVED | CN-office note 五 (rewritten PCT note) has no such reduction; CN "Exemptions, reductions or refunds of fees" lists none |
| Late-entry surcharge | entry after 30 months | — | 1000 | 1 Jul 2017 | MISSING | CN "Surcharge for late entry into the national phase 1,000 CNY" |

Notes: the reduction line `YIELD -ExFee * 0.2 IF (ISA EQ ISA_JP OR ISA EQ ISA_SE OR ISA EQ ISA_EPO) AND IPRP NEQ IPRP_CN`
was deleted; the fee and its VERIFY directives stay. The waiver cases of the three filing-stage
fees now read `CASE RO_NP EQ FALSE OR ISA NEQ ISA_CN` / `CASE RO_NP EQ TRUE AND ISA EQ ISA_CN`
(complementary, so `VERIFY COMPLETE` still holds). This matters since CNIPA as RO may now use the
EPO as ISA (pilot extended to 2031, eGuide CN). The chapter's exemption text says "the
international search report and the international preliminary report on patentability", the
announcement says "或" (or); the schedule follows the announcement.

## CO: PCT-CO.ipf (COP)

Sources: eGuide CO, valid as from 10 September 2026, "With effect since 13 February 2026" (sources/CO.txt); SIC Resolución 6167 de 2026, "Rige a partir del 13 de febrero de 2026" (sources/CO-office.txt, Normograma compilation); SIC Resolución 75972 de 2026, fees for 2027 (sources/CO-office-2.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-02-13 (effective date of Resolución 6167 de 2026).
Fees last changed: 13 February 2026.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | online | 84840 | 118000 | 13 Feb 2026 | UPDATED (amount) | CO-office "1.1.1.2.2.1 ... Solicitud de patente de invención ... 118.000 \| 146.000"; CO "Filing fee Online 118,000 COP" |
| OFF_ExaminationFee | online, PCT application | 1151240 | 1599500 | 13 Feb 2026 | UPDATED (amount) | CO-office "1.1.1.2.2.5 ... examen de patentabilidad ... 1.599.500"; CO "Online 1,599,500 COP" |
| Additional claim | each claim from the 11th | — | 59000 online | 13 Feb 2026 | MISSING | CO-office "1.1.1.2.2.3 ... Reivindicación adicional ... 59.000" |
| Examination reduction | natural persons without means, MiPymes, universities, research non-profits | — | 25 % | 13 Feb 2026 | MISSING | CO-office "1.1.14 ... reducción del veinticinco por ciento (25%)" |

Notes: announced change from 1 January 2027 (not applied): Resolución 75972 de 2026 "empieza a
regir a partir del primero (1) de enero de 2027": filing 126,000 online, examination 1,710,500
online (CO-office-2 items 1.1.1.2.2.1 and 1.1.1.2.2.5). The amounts used are the online rates;
paper rates are 146,000 and 1,919,000.

## CR: PCT-CR.ipf (USD)

Sources: eGuide CR, valid as from 1 January 2026 (sources/CR.txt). The Registro Nacional's own fee table could not be found.
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2026-01-01 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | standard / natural person, micro or small enterprise (−70 %) | 500/150 | 500/150 | not stated | MATCH | CR "Filing fee including formal examination ... 500 USD"; "reduced by 70% ... natural persons, micro or small enterprises" |
| OFF_ExaminationFee | optional | 525 | 525 | not stated | MATCH | CR "Examination fee 525 USD" |
| OFF_RegistrationFee | optional; standard / reduced (−70 %) | 500/150 | 500/150 | not stated | MATCH | CR "Registration fee ... 500 USD" |

Notes: annual fees from the 2nd year are paid within two months of grant (CR.05), consistent with
the schedule's comment.

## CU: PCT-CU.ipf (CUP)

Sources: eGuide CU, valid as from 7 March 2025 (sources/CU.txt). The OCPI's own fee table could not be found (searches returned only Resolución 2416/2003 and 151/2018 without amounts).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE 2025-03-07 (chapter's "Valid as from" date).
Fees last changed: not stated.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always (incl. publication and 1st–2nd annuities) | 11040 | 11040 | not stated | MATCH | CU "Filing fee (including publication fee and the annual fee for the 1st and the 2nd years) 11,040 CUP" |
| OFF_PriorityFee | per priority | 1200 | 1200 | not stated | MATCH | CU "Fee for claiming priority, per priority 1,200 CUP" |
| OFF_SheetFee | each page over 30 | 120 | 120 | not stated | MATCH | CU "Fee for each page in excess of 30 120 CUP" |
| OFF_ClaimFee | each independent claim over one | 1920 | 1920 | not stated | MATCH | CU "for each independent claim in excess of one 1,920 CUP" |
| OFF_ClaimFee | each multiple dependent claim | 2400 | 2400 | not stated | MATCH | CU "for each multiple dependent claim 2,400 CUP" |
| OFF_ClaimFee | each group of up to ten dependent claims | 1920 | 1920 | not stated | MATCH | CU "for each group up to ten dependent claims 1,920 CUP" |
| OFF_Renewal3 | 3rd annuity | 4800 | 4800 | not stated | MATCH | CU "for the 3rd year 4,800 CUP" |
| OFF_GrantFee | optional | 4800 | 4800 | not stated | MATCH | CU "Fee for grant 4,800 CUP" |
| Natural-person / public-university reduction | fees −40 %, annuities from the 3rd −50 % | — | 40 % / 50 % | not stated | MISSING | CU "Fees are reduced by 40% where the applicant is a natural person or a public university" |
| Paper-plus-electronic filing reduction | filing fee | — | −1,200 | not stated | MISSING | CU "reduced by 1,200 CUP where applications are filed both on paper and in electronic form" |

Notes: the 3rd annuity is charged from 12 months after the application date; CU.07 says annual
fees "must be paid within 30 months from the priority date", so at a normal entry it is due anyway.
Condition unchanged.

## Batch summary

| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
|---|---|---|---|---|---|---|---|---|
| BH | 4 | 0 | 3 | 0 | 0 | 1 | 2023-09-22 (office schedule) | 22 Sep 2023 |
| BN | 3 | 2 | 0 | 0 | 0 | 1 | 2025-03-07 (chapter) | not stated |
| BR | 8 | 0 | 6 | 0 | 0 | 2 | 2025-08-07 (office table) | 7 Aug 2025 |
| BW | 5 | 5 | 0 | 0 | 0 | 0 | 2026-08-01 (chapter) | not stated |
| BY | 7 | 0 | 5 | 0 | 0 | 2 | 2026-01-01 (base value 45 BYN) | 1 Jan 2026 |
| BZ | 5 | 3 | 2 | 0 | 0 | 0 | 2022-03-01 (office schedule) | 1 Mar 2022 |
| CA | 6 | 0 | 4 | 0 | 0 | 2 | 2026-01-01 (office table) | 1 Jan 2026 |
| CH | 4 | 3 | 0 | 0 | 0 | 1 | 2019-07-01 (office table) | 1 Jul 2019 |
| CN | 10 | 7 | 1 | 1 | 0 | 1 | 2026-10-01 (chapter) | 6 Aug 2024 (conditions); 1 Jul 2017 (amounts) |
| CO | 4 | 0 | 2 | 0 | 0 | 2 | 2026-02-13 (office resolution) | 13 Feb 2026 |
| CR | 3 | 3 | 0 | 0 | 0 | 0 | 2026-01-01 (chapter) | not stated |
| CU | 10 | 8 | 0 | 0 | 0 | 2 | 2025-03-07 (chapter) | not stated |
| **All** | **69** | **31** | **23** | **1** | **0** | **14** | | |
