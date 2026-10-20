# Audit of official fees in the IPFees corpus (October 2026)

Fifteen offices of the reference corpus (`ipfees` `428abd9`, `src/IPFLang.Engine/Corpus`) were
compared fee by fee with each office's current official fee table and with the WIPO PCT
Applicant's Guide National Chapter, on 19 October 2026. The corpus was not changed. The offices
are the eight that US-origin applications enter most (US, EP, CN, JP, CA, KR, AU, IN; Mathison,
2022), the running example's two (RO, CH), and five more for regional spread (BR, MX, GB, IL, SG).
Translation and agent fees are placeholders and were not audited.

The checks were made by research assistants working read-only from the corpus files and the
public sources named in each table, one batch of five offices each; each batch report is kept as
returned. Every figure cited in the paper must be traceable to a row below.

## Batch 1: US, EP, JP, CN, KR

Office sources were read as text: the USPTO page, the EPO Rules relating to Fees (PDF), the JPO and
KIPO pages through a browser, and CNIPA pages with verbatim Chinese quotes. The WIPO eGuide
chapters were read through a summarising fetch tool; their figures agree with the office sources
in every case.

Sources: USPTO-FS https://www.uspto.gov/learning-and-resources/fees-and-payment/uspto-fee-schedule;
USPTO-old https://www.uspto.gov/sites/default/files/documents/Patents-fees-effective-through_1-18-25.pdf;
RFees https://www.epo.org/en/legal/epc/2020/EPC_rfees_20260401_en_20260326.pdf;
OJ-A2 https://www.epo.org/en/legal/official-journal/2026/01/a2;
JPO https://www.jpo.go.jp/e/system/process/tesuryo/hyou.html and https://www.jpo.go.jp/system/process/tesuryo/hyou.html;
CNIPA-244 https://www.cnipa.gov.cn/art/2020/11/17/art_2468_154949.html;
CNIPA-594 https://www.cnipa.gov.cn/art/2024/8/6/art_74_194102.html;
CNIPA-272 https://www.cnipa.gov.cn/art/2018/6/15/art_527_147400.html;
KIPO https://kipo.go.kr/en/HtmlApp?c=92004;
WIPO-xx https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=XX&doc-lang=en.

### US: PCT-US.ipf (USD, large / small / micro)

Current schedule: USPTO-FS, "Effective January 19, 2025 (Last revised August 14, 2026)"; WIPO-US
applicable from 20 August 2026, last updated 13 October 2026. Every corpus amount equals USPTO-old,
"Effective through January 18, 2025".

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Basic national 1631 | always | 320/128/64 | 350/140/70 | 19 Jan 2025 | DIFFERENT | FS, WIPO-US |
| Search 1641 | ISA=US | 140/56/28 | 150/60/30 | 19 Jan 2025 | DIFFERENT | FS |
| Search 1642 | ISR from another ISA | 540/216/108 | 580/232/116 | 19 Jan 2025 | DIFFERENT | FS |
| Search 1632 | no ISR | 700/280/140 | 770/308/154 | 19 Jan 2025 | DIFFERENT | FS |
| Search 1.492(b)(1) | corpus: any IPRP by US | 0 | 0 only if all claims satisfy Art. 33(1)–(4) | — | MATCH (corpus condition too broad) | FS |
| Exam 1633 | otherwise | 800/320/160 | 880/352/176 | 19 Jan 2025 | DIFFERENT | FS |
| Exam 1.492(c)(1) | corpus: any IPRP by US | 0 | 0 only if US was ISA/IPEA and all claims satisfy Art. 33(1)–(4) | — | MATCH (condition too broad) | FS |
| Size fee 1681 | each 50 sheets >100 | 420/168/84 | 450/180/90 | 19 Jan 2025 | DIFFERENT (threshold OK) | FS |
| Independent claims 1614 | each >3 | 480/192/96 | 600/240/120 | 19 Jan 2025 | DIFFERENT | FS |
| Claims 1615 | each >20 | 100/40/20 | 200/80/40 | 19 Jan 2025 | DIFFERENT | FS |
| Multiple dependent claim 1616 | corpus: × number of such claims | 860/344/172 each | 925/370/185 per application | 19 Jan 2025 | DIFFERENT (amount and basis) | FS, WIPO-US |
| Late search/exam/oath surcharge 1617 | after commencement | — | 170/68/34 | 19 Jan 2025 | MISSING | FS |
| Late English translation 1618 | after 30 months | — | 150/60/30 | 19 Jan 2025 | MISSING | FS |

The US file is exactly the superseded schedule: 9 amounts differ, 2 surcharges are missing, and the
multiple-dependent-claim fee is charged per claim instead of once per application. The 60 % / 80 %
small and micro reductions are modelled correctly.

### EP: PCT-EP.ipf (EUR)

Corpus header: "Valid until 31/03/2023". Current rules: Rules relating to Fees in force from
1 April 2026 (CA/D 9/25, OJ EPO 2026, A2); WIPO-EP, applicable from 17 September 2026, gives the
same figures.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Filing | Form 1200 filed online | 135 | 135 | unchanged 1 Apr 2026 | MATCH | RFees Art. 2(1) item 1 |
| Filing | not online | — | 285 | 1 Apr 2026 | MISSING | RFees item 1 |
| Designation | always | 660 | 720 | 1 Apr 2026 | DIFFERENT | RFees item 3 |
| Extension, Bosnia and Herzegovina | requested | 102 | 102 | — | MATCH | WIPO-EP |
| Page fee | each page >35 | 17 | 17 | — | MATCH | RFees item 1a |
| Claims | 16th–50th | 265 | 290 | 1 Apr 2026 | DIFFERENT | RFees item 15 |
| Claims | 51st+ | 660 | 720 | 1 Apr 2026 | DIFFERENT | RFees item 15 |
| Supplementary search | other ISAs | 1,460 | 1,595 | 1 Apr 2026 | DIFFERENT | RFees item 2 |
| Supplementary search | ISA AT/ES/FI/SE/TR/XN/XV | 275 (1,460−1,185) | 230 (1,595 − EUR 1,365); application filed ≤31 Mar 2028, fee paid ≥1 Apr 2026 | 1 Apr 2026 | DIFFERENT | OJ-A2 Art. 3 |
| Supplementary search | ISA=EPO | 0 | not payable | — | MATCH | WIPO-EP |
| Examination | no supplementary search report (ISA=EPO) | 2,055 | 2,240 | 1 Apr 2026 | DIFFERENT | RFees item 6 |
| Examination | supplementary search report drawn up | 1,840 | 2,010 | 1 Apr 2026 | DIFFERENT | RFees item 6 |
| Exam reduction | EPO drew up the IPER | fee × 0.75 (pays 75 %) | "reduced by 75%" (pays 25 %) | — | DIFFERENT (logic inverted) | RFees Art. 14(2) |
| Validation KH/MA/MD/TN | selected | 180/240/200/180 | 180/240/200/180 | — | MATCH ×4 | WIPO-EP |
| Validation GE | — | — | 200 | — | MISSING | WIPO-EP |
| Validation LA | — | — | 180 | — | MISSING | WIPO-EP |
| Renewal, 3rd year | corpus: ≥18 months after filing | 530 | 725 | 1 Apr 2026 | DIFFERENT (amount; trigger doubtful) | RFees item 4, WIPO-EP |
| Rule 7a EPC reduction | micro-entities and others, conditions apply | none | 30 % | 1 Apr 2024 | MISSING | RFees Art. 14(1) |

EP amounts are at least two adjustment rounds old, the IPER reduction is inverted, and Georgia,
Laos and the micro-entity reduction are absent. Under Rule 51(1) EPC the third-year renewal fee
falls due at the second anniversary of filing, not at 18 months (from the assistant's knowledge,
not re-fetched). Third-party summaries give pre-April-2026 values of 1,520 (search), 685
(designation) and 1,915 / 2,135 (examination); the corpus is older than those too.

### JP: PCT-JP.ipf (JPY)

JPO schedule: English page headed "on or after April 1, 2022"; Japanese page updated 6 April 2026.
WIPO-JP applicable from 1 July 2026.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National fee (Patent Act Art. 184-5(1)) | always | 14,000 | 14,000 | schedule from 1 Apr 2022 | MATCH | JPO, WIPO-JP |
| Exam request | no ISR | 138,000 + 4,000/claim | 138,000 + 4,000/claim | international filing date ≥1 Apr 2019 | MATCH | JPO |
| Exam request | ISR by JPO | 83,000 + 2,400/claim | same | same | MATCH | JPO |
| Exam request | ISR by another ISA | 124,000 + 3,600/claim | same | same | MATCH | JPO |
| Exam request | report by a designated search organisation | 111,000 + 3,200 (declared, never yielded) | 110,000 + 3,200 | same | DIFFERENT (unused constant) | JPO |
| Fee reductions | individuals, SMEs, micro enterprises, academic institutions | none | exist (amounts not verified) | — | MISSING | WIPO-JP, JPO |

JP is current for every fee it yields; only an unused constant and the reductions are off.

### CN: PCT-CN.ipf (CNY)

Amounts: 发改价格〔2017〕270号 (CNIPA Announcement 244), in force 1 July 2017. PCT notes rewritten by
CNIPA Announcement 594 (6 August 2024). Announcement 272 stopped only the grant publication fee
(公告印刷费), so the 50 CNY application publication fee (公布印刷费) is still charged. WIPO-CN,
applicable from 1 October 2026, confirms the amounts.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Filing | waived if receiving office = national office | 900 | 900; waived if CNIPA was RO and ISA | 1 Jul 2017 / 6 Aug 2024 | MATCH (condition wording) | 244, 594 |
| Claims | each >10 | 150 | 150 | 1 Jul 2017 | MATCH | 244, WIPO-CN |
| Description pages | 31–300 / >300 | 50/100 | 50/100 | 1 Jul 2017 | MATCH | 244 |
| Publication printing | always | 50 | 50 | 1 Jul 2017 | MATCH | 244, 272, WIPO-CN |
| Priority claim | each | 80 | 80 | 1 Jul 2017 | MATCH | 244 |
| Examination | standard | 2,500 | 2,500 | 1 Jul 2017 | MATCH | 244 |
| Exam exemption | IPRP by CNIPA | 0 | 0 | 6 Aug 2024 | MATCH | 594 |
| Exam exemption | ISR by CNIPA | 2,500 | 0 | 2017, restated 2024 | DIFFERENT | 594 |
| Exam 20 % reduction | ISA EP/JP/SE | 2,000 | absent from the rewritten note, so 2,500 | 6 Aug 2024 | NOT OFFICIAL (removal inferred; WIPO-CN also omits it) | 594 |
| Late-entry grace fee | entry at 31–32 months | — | 1,000 | 1 Jul 2017 | MISSING | 244, WIPO-CN |

CN amounts are current, but the exemption and reduction logic follows the pre-August-2024 notes.
(The instance does not serve CN: the save gate refuses its schedule for three unsettled zero cases.)

### KR: PCT-KR.ipf (KRW)

KIPO/MOIP fee page "in effect since August 1, 2023" (page last updated 10 February 2026); WIPO-KR
applicable from 1 August 2026.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National fee | electronic | 46,000 | 46,000 | 1 Aug 2023 | MATCH | KIPO, WIPO-KR |
| National fee | paper, +1,000/sheet >20 | — | 66,000 + 1,000/sheet | 1 Aug 2023 | MISSING | KIPO |
| Priority claim | each | 18,000 | 18,000 electronic (20,000 paper) | 1 Aug 2023 | MATCH | KIPO |
| Exam request | always | 143,000 + 44,000/claim | 166,000 + 51,000/claim | 1 Aug 2023 | DIFFERENT | KIPO, WIPO-KR |
| 70 % reduction | corpus: natural person | −70 % on filing and exam | 70 % if applicant is a natural person and the inventor; over 20 applications a year, only a 30 % filing-fee reduction | — | MATCH (condition simplified) | WIPO-KR |
| 70 % exam reduction | ISR and/or IPER by MOIP | — | −70 % | — | MISSING | WIPO-KR |

The KR exam fee has been stale since August 2023, and the reduction for a Korean ISR/IPER is absent.

### Official online calculators (batch 1)

USPTO: none found for the national stage. EPO: the Fee Assistant
(https://www.epo.org/en/applying/fees/fee-assistant) "currently relates only to the fees for
European patent applications"; Euro-PCT regional phase coverage not confirmed. JPO: the
手続料金計算システム (https://www.jpo.go.jp/system/process/tesuryo/jidou-keisan/kokunai.html) does
cover national phase entry, with the examination variants and reductions. CNIPA: none found. KIPO:
none found on KIPO or Patent-ro (absence not confirmed).

### Batch 1 summary

| Office | Rows | Match | Different | Not official | Missing |
|---|---|---|---|---|---|
| US | 13 | 2 | 9 | 0 | 2 |
| EP | 21 | 8 | 9 | 0 | 4 |
| JP | 6 | 4 | 1 | 0 | 1 |
| CN | 10 | 7 | 1 | 1 | 1 |
| KR | 6 | 3 | 1 | 0 | 2 |
| Total | 56 | 24 | 21 | 1 | 10 |

Of the 46 corpus fee lines checked, 24 match, 21 differ and 1 has no current official basis;
another 10 official fees or reductions are missing. JP and CN amounts are current; US amounts
predate 19 January 2025, the KR examination fee predates 1 August 2023, and EP amounts predate the
2024 and 2026 rounds. Three logic errors are independent of staleness: the EP IPER
examination-fee reduction is inverted, the US multiple-dependent-claim fee is charged per claim
instead of once per application, and the CN examination-fee exemption when CNIPA prepared the ISR
is missing. Every corpus file carries `VERSION '2024.1' EFFECTIVE 2024-01-01`, which is not
accurate for US, EP or KR.

## Batch 2: IN, CA, AU, BR, MX

All five files declare `VERSION '2024.1' EFFECTIVE 2024-01-01`. Amounts written "a / b" are
reduced / full.

### IN: PCT-IN.ipf (INR, e-filing; the corpus reduces only Entity_Small and Entity_Person)

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Basic national fee (Form 1) | ≤30 sheets, ≤10 claims | 1,600 / 8,000 | 1,600 / 8,000 | PDF undated; WIPO chapter applies from 1 Jul 2026 | MATCH | S1, W1 |
| Sheet fee | each sheet over 30 | 160 / 800 | 160 / 800 | same | MATCH | S1, W1 |
| Claim fee | each claim over 10 | 320 / 1,600 | 320 / 1,600 | same | MATCH | S1, W1 |
| Priority | each priority after the first | 1,600 / 8,000 per priority | "Multiple of 1600 / 8000 in case of every multiple priority" | same | MATCH | S1, W1 |
| Examination request (Form 18, s.11B) | optional | 4,000 / 20,000 | 4,000 / 20,000 | same | MATCH (the file's "48 months" comment is outdated: WIPO IN.12 says 31 months from priority) | S1 |
| Examination request under rule 20(4)(ii) | rule 20 covers PCT applications | — | 5,600 / 28,000 | same | MISSING | S1 |
| Sequence-listing pages | per page | — | 160 / 800, capped at 24,000 / 120,000 | same | MISSING | S1 |

The official reduced rate covers "natural person or startup or small entity or educational
institution"; the corpus charges full fees to Entity_Micro, Less25 and Less100. Paper-filing rates
are not modelled (the file says e-filing only). Sources: S1
https://ipindia.gov.in/frontend/pdf/forms_and_official_fees/Schedule_1.pdf; W1
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=IN&doc-lang=en (applicable from 1 July
2026, last updated 25 September 2026). Every amount is current; two PCT-related fees are missing.

### CA: PCT-CA.ipf (CAD; small entity / standard; the corpus reduces only Entity_Small)

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Basic national fee | — | 203.59 / 407.18 | 241.24 / 595.06 | 1 Jan 2026 | DIFFERENT | S2, W2 |
| Examination | ISA was CIPO | 100 / 203.59 | 117.94 / 297.00 | 1 Jan 2026 | DIFFERENT | S2, W2 |
| Examination | any other ISA | 407.18 / 814.37 | 482.48 / 1,190.13 | 1 Jan 2026 | DIFFERENT | S2, W2 |
| Maintenance fee | 2nd–4th anniversary | 50 / 100 | 60.26 / 134.02 | 1 Jan 2026 | DIFFERENT | S2, W2 |
| Excess claims | each claim over 20 | — | 58.97 / 117.94 | 1 Jan 2026 | MISSING | S2 |
| Late national entry (up to 42 months) | reinstatement plus late fee | — | 297.00 + 150.00 | 1 Jan 2026 | MISSING | S2, W2 |

CIPO indexes its fees every year and already publishes the 2027 amounts (from 1 January 2027:
basic national fee 245.34 / 605.18; examination 119.94 / 302.05 with CIPO as ISA, 490.68 / 1,210.36
otherwise; excess claims 59.97 / 119.94; maintenance 61.28 / 136.30). Small-entity eligibility
covers "entities with fewer than 100 employees and universities"; the corpus charges standard fees
to Less25, Less100 and Micro. The ISA condition matches CIPO's. Sources: S2
https://ised-isde.canada.ca/site/canadian-intellectual-property-office/en/node/39; W2
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=CA&doc-lang=en (applicable from 31 August
2026). Every amount is out of date, and excess claims are missing.

### AU: PCT-AU.ipf (AUD; no entity reductions exist)

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National phase entry | — | 370 | 400 online / 600 other means | 1 Oct 2024 | DIFFERENT (370 is the pre-October-2024 amount) | S3, S4, W3 |
| Examination | IP Australia did the international preliminary examination | 300 | 350 | 1 Oct 2024 | DIFFERENT | S3, W3 |
| Examination | other cases | 490 | 550 | 1 Oct 2024 | DIFFERENT | S3, W3 |
| Excess claims at first report | 21st–30th claim; over 30 | — | 125 each; 250 each | 1 Oct 2024 (new fee) | MISSING | S3, S5 |
| Acceptance | minimum | — | 300, plus 125 / 250 for claims added | 1 Oct 2024 | MISSING | S3, S5 |
| Continuation fees | from 4th anniversary of the international filing date | — | 300 (4th), 315 (5th), 345 (6th)… online | 6th onward changed 1 Oct 2024 | MISSING | S3, W3 |

The `IPRP EQ IPRP_AU` condition matches the official wording. IP Australia has a trade mark price
calculator only. Sources: S3 IP Australia, summary table of fee changes
(https://www.ipaustralia.gov.au/about-us/accountability-and-reporting/~/-/media/Project/IPA/IPAustralia/PDF/Accountability-and-reporting/CRIS/Summary-table-of-fee-changes.pdf?rev=91702b2c6e9140419b18bd7f01bcea67);
S4 https://www.ipaustralia.gov.au/international-ip/international-cooperation/the-patent-cooperation-treaty;
S5 https://www.ipaustralia.gov.au/patents/understanding-patents/time-and-costs; W3
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=AU&doc-lang=en (applicable from 20 August
2026). The corpus still holds the pre-October-2024 schedule.

### BR: PCT-BR.ipf (BRL; official discounted amounts in parentheses; the corpus has no discount)

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National phase entry (code 200) | electronic | 175 | 260 (130); paper 380 | 7 Aug 2025 | DIFFERENT | S6, S7, W4 |
| Examination (code 203) | up to 10 claims | 590 | 870 (435) | same | DIFFERENT | S6, W4 |
| Claim surcharge | each of claims 11–15 | 100 | 140 (70) | same | DIFFERENT | S6 |
| Claim surcharge | each of claims 16–30 | 200 | 290 (145) | same | DIFFERENT | S6 |
| Claim surcharge | each claim from the 31st | 500 | 740 (370) | same | DIFFERENT | S6 |
| Annuities | corpus: year 1 = 295, year 2 = 295, year 3 = 780 | as left | Application annuity (code 220): 400 (200); WIPO ties the first one to the 24-month point from the international filing date | same | DIFFERENT (no annuities are due for years 1–2) | S6, W4 |
| Examination via PCT (code 284) | INPI was the ISA/IPEA | — | 570 (285), plus the same claim surcharges | same | MISSING | S6 |
| 50 % discount | natural persons, ME/MEI/EPP, ICTs, non-profits, public bodies | — | 50 % | same | MISSING | S6 |

The claim thresholds (10, 15, 30) are correct. The table is "Portaria GM/MDIC nº 110/2025,
Portaria INPI/PR nº 10/2025 e apostila 31/10/2025", in force since 7 August 2025. Sources: S6
https://www.gov.br/inpi/pt-br/servicos/patentes/NovaTabeladeRetribuiesINPI_Patentes_20_dez_25.pdf;
S7 https://www.gov.br/inpi/pt-br/servicos/custos-e-pagamento; W4
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=BR&doc-lang=en (applicable from 1 October
2026). The 2025 fee table superseded every amount in the file.

### MX: PCT-MX.ipf (MXN, excluding 16 % IVA)

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National fee, Chapter I (Art. 1b) | OfficeElected = FALSE | 3,147 | 3,147.00 | tariff last reformed DOF 15-03-2024 | MATCH | S8, W5 |
| National fee, Chapter II (Art. 1c) | OfficeElected = TRUE | 1,500 | 1,500.00 | same | MATCH | S8, W5 |
| Sheet fee (Arts. 1b bis, 1c bis) | each sheet over 30 | 61 | 61.00 | same | MATCH | S8, W5 |
| Priority (Art. 28) | each priority claimed | 1,066.17 × n | 1,066.17 each | same | MATCH | S8, W5 |
| 50 % reduction (Disposición CUARTA) | inventors who are natural persons, micro/small industries, universities, public research institutes; covers Arts. 1a–1f but not Art. 28 | — | 50 % | same | MISSING | S8 |

IVA applies under the tariff's SEXTA provision. The corpus correctly leaves out annuities, which
are not due until grant. The file's comment that the discount was "not clear from WIPO page" is
superseded: the tariff confirms it. No reform later than 15-03-2024 was found, and the WIPO chapter
(applicable from 1 January 2026) shows the same amounts. Sources: S8
https://www.gob.mx/cms/uploads/attachment/file/959396/1._Acuerdo.Tarifa_15.03.24.pdf; W5
https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=MX&doc-lang=en. Every amount is current;
only the reduction is missing.

### Batch 2 summary

22 fee rows checked: 9 match (IN 5, MX 4), 13 differ (CA 4, AU 3, BR 6), 10 missing (IN 2, CA 2,
AU 3, BR 2, MX 1), 0 not official. Offices with stable schedules (India, Mexico) are current; those
that revised their fees (Australia on 1 October 2024, Brazil on 7 August 2025) or index them yearly
(Canada, each 1 January) are stale, so the files' `EFFECTIVE 2024-01-01` claim does not hold for
them. Reductions for natural persons and small entities are absent (Brazil, Mexico) or partly
mapped (India, Canada); in Australia a claim-dependent fee is missing altogether. None of the five
offices publishes an official online calculator covering national phase entry. Caveats: the IP
India schedule carries no effective date; for Mexico a later DOF reform could not be ruled out; the
WIPO chapters were read through a summarising fetch tool, and the figures come from the office
sources, which agree with WIPO.

## Batch 3: GB, IL, RO, CH, SG

All five files are labelled `VERSION '2024.1' EFFECTIVE 2024-01-01`. The WIPO PCT Applicant's
Guide national chapters are at `https://pctlegal.wipo.int/eGuide/view-doc.xhtml?doc-code=XX&doc-lang=en`.

### GB: PCT-GB.ipf (GBP)

UK source: S.I. 2026/183 (https://www.legislation.gov.uk/uksi/2026/183/made) and the revised
Patents (Fees) Rules 2007 r.3(2) (https://www.legislation.gov.uk/uksi/2007/3292/rule/3): "The
prescribed fee to begin the national phase … is £40." The gov.uk table of current and new fees
(https://www.gov.uk/csv-preview/698b663346be5092a1cfd89c/Patents_list_of_current_and_new_fees___4_.csv)
gives the same figures. The WIPO GB chapter, "Applicable as from April 1, 2026" and last updated
1 May 2026, agrees on every amount.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National fee (NP1) | always | 30 | 40 | 1 Apr 2026 | DIFFERENT | SI 2026/183 r.6(b); WIPO GB |
| Excess claims (Form 9A) | per claim >25 | 20 | 27 | 1 Apr 2026 | DIFFERENT (threshold matches) | SI 2026/183 r.8 |
| Excess pages (Form 10) | per description page >35 | 10 | 13 | 1 Apr 2026 | DIFFERENT (threshold matches) | SI 2026/183 r.9 |
| Search (9A) | ISA already searched | 120 | 160 online / 200 paper | 1 Apr 2026 | DIFFERENT | gov.uk CSV; WIPO GB |
| Search (9A) | other cases | 150 | 200 online / 240 paper | 1 Apr 2026 | DIFFERENT | gov.uk CSV; WIPO GB |
| Examination (Form 10) | optional, due by 33 months | 130 | 130 online / 170 paper | 1 Apr 2026 | MATCH (online rate) | SI 2026/183 r.15 |
| Publication of translation (NP1) | non-English applications only | — | 16 | 1 Apr 2026 | MISSING FROM CORPUS | gov.uk CSV |

The April 2026 increase left the file out of date on 5 of 6 amounts. The examination match looks
coincidental: 130 was the old paper rate, while the file's search fees follow the old online
rates. No SME or natural-person reductions exist; the only reduction is a GBP 40 cut for
electronic filing.

### IL: PCT-IL.ipf (ILS)

The Israel Patent Office's own fee pages (gov.il) returned HTTP 403. The figures come from the
WIPO IL chapter, "Applicable as from August 1, 2026" and last updated 11 June 2026; a law-firm
guide on Mondaq (https://www.mondaq.com/ip/1731204/patents-comparative-guide) gives the same 2026
amounts. The new amounts are about 10 % above the corpus.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Filing fee | always | 2183 | 2,402 | 2026 (sources conflict: 1 Jan vs 1 Mar 2026; WIPO version 1 Aug 2026) | DIFFERENT | WIPO IL Annex IL.I |
| Size fee | per extra 50 sheets beyond 100 | 273 | 300 | same | DIFFERENT (threshold matches) | WIPO IL |
| Claims fee | per claim from the 51st | 560 | 616 | same | DIFFERENT (threshold matches) | WIPO IL |

All three amounts are stale (pre-2026 index level); the structure is right. WIPO lists a 40 %
reduction for individuals and for companies with turnover under ILS 10 million, but only for
applications first filed in Israel, which probably excludes national-phase entries (not confirmed).
WIPO's own Summary says "None" for reductions, which conflicts with its annex.

### RO: PCT-RO.ipf (EUR)

Source: OSIM Order 79/01.10.2025, Annex 1 (https://osim.ro/images/Ordine_Dispozitii/ordin_79_01.10.2025.pdf),
applicable from 1 January 2026 at 1 EUR = 5.0811 lei, with the same EUR column as the 2025 annex.
The WIPO RO chapter, "Applicable as from January 1, 2026" and last updated 14 April 2026, matches.
Foreign applicants pay in EUR, so the corpus currency is right for them.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Opening of national phase (item 26) | always | 30 | 30 (152 lei) | in force; unchanged 2025→2026 | MATCH | OSIM Annex 1; WIPO RO |
| Priority claim at national phase entry (item 8b) | per priority | 50 | 50 (254 lei) | same | MATCH | OSIM Annex 1 |
| Examination, fast tier (item 7) | ≤20 pages, ≤5 claims | 500 | 500 (2,541 lei) | same | MATCH | OSIM Annex 1 |
| – per page above 20 | | 10 | 10 | same | MATCH | OSIM Annex 1 |
| – per claim above 5 | | 15 | 15 | same | MATCH | OSIM Annex 1 |
| Examination, standard tier (item 3) | requested in months 4–30 | — | 300 + 5 per page >20 + 10 per claim >5 | same | MISSING FROM CORPUS | OSIM Annex 1; WIPO "after three months" |
| Publication of Romanian translation (item 27) | within legal term / before it | — | 50 / 80 | same | MISSING FROM CORPUS | OSIM Annex 1 |
| Search report for a national-phase international application (item 6b) | on request | — | 50 | same | MISSING FROM CORPUS | OSIM Annex 1; WIPO RO |

Every amount in the file matches, but the file models only the 500 EUR fast examination tier and
lacks the translation-publication and search fees. The comment on `OFF_PriorityFee` about
"3 months" belongs to the examination fee. Current reductions under OG 41/1998 could not be
verified (legislatie.just.ro returned HTTP 502); WIPO lists only the 50 % search reduction.

### CH: PCT-CH.ipf (CHF)

Sources: the IPI fees page (https://www.ige.ch/en/protecting-your-ip/patents/before-you-apply/costs-and-fees/patents-fees,
"Status per 1 July 2019") and the IPI overview of new fees dated 20 May 2026
(https://www.ige.ch/fileadmin/user_upload/schuetzen/patente/e/UEbersicht_neue_Gebuehren_CH-Patent_2027_en.pdf).
The WIPO CH chapter, "Applicable as from April 4, 2024" and last updated 8 August 2024, agrees.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| Filing fee | always | 200 | 200 | 1 Jul 2019 (valid until 31 Dec 2026) | MATCH | IPI; WIPO CH |
| Claims fee | per claim from the 11th | 50 | 50 | same | MATCH | IPI; WIPO CH |
| Examination fee | optional; due on IPI invitation | 500 | 500 | same | MATCH | IPI; WIPO CH |
| Search | optional | — | 500 | same | MISSING FROM CORPUS (optional; not in the WIPO annex) | IPI 2027 overview |

Correct today, out of date from 1 January 2027: the claims fee then starts at the 16th claim,
examination drops to 400, full substantive examination costs 300 more, and search becomes
mandatory at 500 (transitional rules not read). Accelerated examination (CHF 200) is absent. No
reductions are listed.

### SG: PCT-SG.ipf (SGD)

Sources: the IPOS forms and fees page (https://www.ipos.gov.sg/about-ip/patents/managing-patents/forms-fees,
"Last updated on 9 October 2026") and the WIPO SG chapter, "Applicable as from June 1, 2026" and
last updated 7 May 2026.

| Fee | Condition | Corpus | Official now | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| National phase entry (PF37) | always | 200 | 210 | in force (no date given) | DIFFERENT | IPOS; WIPO SG |
| Examination report (PF12) | base fee | 1350 | 1,420 | in force | DIFFERENT | IPOS; WIPO SG |
| Excess claims (with PF12) | per claim above threshold | 40 per claim >20 | 80 per claim >15 | requests filed on or after 1 Sep 2025 | DIFFERENT (amount and threshold) | IPOS; WIPO SG |
| Combined search and examination (PF11) | IPOS issued the ISR/IPRP / other cases | — | 1,750 / 2,050 + 80 per claim >15 | 1 Sep 2025 | MISSING FROM CORPUS | IPOS; WIPO SG |

All three amounts are stale, including the claim threshold, and the file has no PF11 route. Not
modelled either: excess claims charged at each response to a written opinion since 1 April 2026
(PF13A, SGD 80 per claim), and a flat SGD 210 grant fee for requests filed on or after 1 September
2025. WIPO lists no reductions.

### Official online calculators (batch 3)

None of the five offices publishes an official online fee calculator covering national phase
entry (UKIPO, IPI, IPOS and OSIM publish tables; the Israel Patent Office's pages could not be
reached).

### Batch 3 summary

20 corpus amounts checked: 9 match (1 GB, 5 RO, 3 CH), 11 differ (5 GB, 3 IL, 3 SG), 0 not
official. 6 official fees are missing (1 GB, 3 RO, 1 CH, 1 SG), not counting grant, renewal,
extension and acceleration fees. The differences follow fee changes after the files' stated
2024-01-01 baseline: the UK on 1 April 2026, Israel's 2026 indexation, Singapore on 1 September
2025. Only Romania and Switzerland are current, and Switzerland's figures expire on 1 January 2027.

## Overall

| Batch | Offices | Corpus lines checked | Match | Differ | Not official | Missing official fees |
|---|---|---|---|---|---|---|
| 1 | US, EP, JP, CN, KR | 46 | 24 | 21 | 1 | 10 |
| 2 | IN, CA, AU, BR, MX | 22 | 9 | 13 | 0 | 10 |
| 3 | GB, IL, RO, CH, SG | 20 | 9 | 11 | 0 | 6 |
| **All** | **15 offices** | **88** | **42 (48 %)** | **45 (51 %)** | **1** | **26** |

Six offices are current in every amount they compute: RO, CH (until 1 January 2027), IN, MX, JP
and CN. Nine have changed fees since the corpus baseline that every file declares
(`VERSION '2024.1' EFFECTIVE 2024-01-01`): US (19 January 2025), EP (1 April 2024 and 1 April 2026),
KR (examination fee, 1 August 2023), CA (yearly indexation, 1 January), AU (1 October 2024),
BR (7 August 2025), GB (1 April 2026), IL (2026 indexation) and SG (1 September 2025). Almost every
difference is an amount that changed while the structure around it (the fees, their conditions,
thresholds and currencies) still holds.

Four differences are modelling errors rather than stale amounts: the EP examination-fee reduction
for an EPO preliminary report is inverted (75 % charged instead of 25 %); the US
multiple-dependent-claim fee is charged per claim instead of once per application; the CN
examination exemption when CNIPA drew up the search report is missing; and the BR schedule charges
annuities for years 1 and 2, which are not yet due at national phase entry. Several reductions for
natural persons and small entities are absent or only partly mapped (EP, JP, KR, IN, CA, BR, MX).

Only the JPO and, for European applications, the EPO publish an official calculator; none of the
other thirteen offices does for national phase entry.

## Rows at IPFees `beab6fd`, the version the paper describes

The paper describes IPFees `beab6fd`. Four rows read differently there than at `428abd9`, against
the same official sources, and the paper's Table 3 counts them as they read at `beab6fd`:

| Office | Row | Corpus at `beab6fd` | Verdict |
|---|---|---|---|
| EP | Exam reduction, EPO drew up the IPER | fee × 0.25 (pays 25 %) | MATCH |
| US | Multiple dependent claim 1616 | 860/344/172 per application | DIFFERENT (amount) |
| CN | Exam exemption, ISR by CNIPA | 0 | MATCH |
| BR | Annuities | year 3 = 780 from the 24-month point; none for years 1 and 2 | DIFFERENT (amount) |

Totals at `beab6fd`: 44 match, 43 differ and 1 not official (the CN 20 % reduction), 26 missing;
Table 3 counts the not-official line with the 44 that differ. EP has 9 matching and 8 differing
lines, CN 8 and 1.
