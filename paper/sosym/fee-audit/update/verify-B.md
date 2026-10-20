# Independent check B: offices L–Z, EP, EA

Checked on 20 October 2026 against `corpus-update.diff`, the records `batch-03/06/07/08/09/10.md`,
the saved sources in `sources/`, and the schedules in `ipfees/src/IPFLang.Engine/Corpus/`.
Nothing was edited except this file.

Scope: every changed line in the diff for EA, EP and LA…ZW that carries an amount, a per-unit
rate, a threshold, a condition or a currency. Version lines, comment-only lines, the
ApplicationDate/SheetCount label changes and VERIFY directive changes were not counted. A
"value" below is one fee under one condition; the entity-size variants of the same row
(e.g. US 350/140/70) count as one value, as in the protocol.

Offices in scope with no changed value (only version/comment/label lines, or no diff at all):
LA, LK, LR, LS, LU, MA, MD, MG, MK, MN, MW, MX, MZ, NA, NG, NZ, OM, PA, PE, PG, PH, PL, RO, SA,
SC, SD, TH, TJ, TN, TT, TZ, UA, UG, UZ, VC, VN, WS, ZA, ZM, ZW. For these only the MATCH
spot-check applies (section 3).

**Totals: 64 changed values checked; 59 confirmed (source and record both agree); 5 not
confirmed (see Discrepancies).**

## 1. Changed values, office by office

### EA (PCT-EA.ipf, RUB): 8 values, all OK
- OFF_BasicNationalFee 36000 → 70000: OK. EA-office.txt l.112 "единая процедурная пошлина за подачу евразийской заявки – в размере 70000 рублей"; EA.txt "Unitary procedural fee ... 70,000 RUB". The 0.75 multiplier for ISR is unchanged and matches EA-office l.130 "уменьшаются на 25 процентов".
- OFF_ExaminationFee base 40000 → 70000: OK. EA-office l.212 "(i) в отношении одного изобретения – в размере 70000 рублей".
- OFF_ExaminationFee 2nd independent claim, `Fee` → literal 40000 (EQ 2 and GT 2 lines): OK, amount unchanged. EA-office l.213 "дополнительно 40000 рублей за второй независимый пункт".
- IndepClaimFee 13000 → 25000: OK. EA-office l.214 "по 25000 рублей за каждый последующий независимый пункт".
- Claim fee CF3 (6th–20th) 4800 → 7000, CF2 (21st–50th) 5200 → 7500, CF1 (51st+) 6500 → 8000: OK. EA-office l.113–116 "свыше пятого ... 7000 рублей; свыше двадцатого ... 7500 рублей; свыше пятидесятого ... 8000"; band reading (each band at its own rate) matches the schedule.
- OFF_GrantFee 25000 → 45000: OK. EA-office l.249 "пошлина в размере 45000 рублей".
- Record rows agree with the schedule. Effective date: EA-office l.136 "Вводятся в действие с 1 февраля 2026 г."

### EP (PCT-EP.ipf, EUR): 9 values, 8 OK, 1 discrepancy
- OFF_DesignationFee 660 → 720: OK. EP-office-2.txt item 3 "Designation fee ... 720".
- ClaimFee CF1 265 → 290 (16th–50th): OK. EP-office-2 item 15 "up to the limit of 50 / 290".
- ClaimFee CF2 660 → 720 (51st+): OK. EP-office-2 item 15 "for the 51st and each subsequent claim / 720".
- SearchFee SF1 1460 → 1595: OK. EP-office-2 item 2 "a European or supplementary European search ... 1 595" (thousands separator is a space; read correctly).
- SearchFee reduction SF2 1185 → 1365: OK. EP-office-2 Art. 3 "is reduced by EUR 1 365"; EP.txt "reduced by 1,365 EUR". ISA list in the schedule (AT, FI, NPI, ES, SE, TR, VI) matches Art. 3.
- ExFee1 2055 → 2240 (no supplementary search): OK. EP-office-2 item 6 "no supplementary European search report is drawn up ... 2 240".
- ExFee2 1840 → 2010 (supplementary search drawn up): OK. EP-office-2 item 6 "... supplementary European search report is drawn up ... 2 010". Not swapped: ExFee1 is used when ISA = EPO (no supplementary search).
- OFF_Renewal3 530 → 725: OK. EP-office-2 item 4 "for the 3rd year / 725".
- OFF_Renewal3 threshold `RenMonth GTE 18` → `GTE 24`: **supported by the source but contradicts the record** (see Discrepancies D1). EP.txt l.410 "due before the expiration of the month containing the second anniversary (24 months) of the international filing date".

### NI (PCT-NI.ipf): 2 values, both not confirmed
- OFF_BasicNationalFee 200 → 240 and currency NIO → USD (`# Currency` header and `RETURN Currency`): see Discrepancies D2. The figure "$CA240.00" is in NI-office.txt ("Por las primeras 20 reivindicaciones $CA240.00"), but no saved source equates the Central American peso with the US dollar.

### NO (PCT-NO.ipf, NOK): 3 values, all OK
- OFF_BasicNationalFee 4650 → 6050: OK. NO-office.txt "Application to extend a PCT application to Norway / Companies with more than 20 full-time employees ... | NOK 6 050"; NO.txt "6,050 NOK (1,100 NOK)". Large-entity rate, as at baseline.
- OFF_ClaimFee 250 → 330 per claim > 10: OK. NO-office "Additional fee for each new patent claim over ten | NOK 330" (PCT-entry block; the national-filing block says 325, correctly not used).
- OFF_Renewal 700 → 910 (years 1–3): OK. NO-office "1st fee year | NOK 910", "2nd ...910", "3rd ... 910"; NO.txt "per year 910 NOK".

### OA (PCT-OA.ipf, XAF): 4 values, all OK
- OFF_PriorityFee 63000 → 75000 per priority: OK. OA-office "c) Claim of priority, by priority | 75 000"; OA.txt "75,000 XAF".
- OFF_ClaimFee 45000 → 65000 per claim > 10: OK. OA-office "i) Supplement per claim beyond the 10th | 65 000".
- OFF_Renewal 2nd year and 3rd year 220000 → 270000: OK. OA-office "From year 2 to year 5, by year | 270 000"; OA.txt "270,000 XAF".

### PT (PCT-PT.ipf, EUR): 1 value, OK (open source conflict noted)
- OFF_BasicNationalFee 54.54 → 64.21: OK. PT-office.txt (Deliberação 334/2026, "com efeitos a 1 de julho de 2026"), section "Pedido internacional de patente (PCT)": "Entrada em fase nacional 64,21 € 128,42 €" (online / paper). Decimal comma read correctly; online rate used as the protocol asks. The record already flags that PT.txt gives the national filing fee instead ("Filing fee (includes publication and examination) ... 128.42 EUR"); this is an open decision, not an error.

### QA (PCT-QA.ipf, QAR): 1 value, OK
- OFF_BasicNationalFee 2000/1000 → 5000/2500: OK. QA.txt l.219 "— in case of filing by companies, institutions and research entities 5,000 QAR — in case of filing by individuals 2,500 QAR". Mapping to `Entity_Person` for the lower rate is the baseline mapping.

### RS (PCT-RS.ipf, RSD): 4 values, all OK
- OFF_BasicNationalFee 8210 → 10660: OK. RS-office.txt Тарифни број 107 "За међународну пријаву патента ... 1) до десет патентних захтева 10660 5330". The natural-person column (5330) is exactly half, matching the schedule's −50 % for Entity_Person.
- OFF_ClaimFee 800 → 1040: OK. Same block "2) за сваки следећи патентни захтев преко десет ... 1040 520".
- OFF_SearchFee 16470 → 21390: OK. Тарифни број 110 "ако не постоји извештај о међународном претраживању ... 21390 10695".
- OFF_Renewal3 11510 → 14950: OK. Тарифни број 111 "1) трећу годину 14950 7475".
- That the page carries the 1 July 2026 level is supported: RS-office "Тарифни број 1 ... 430" and "Тарифни број 9 ... 740" equal RS-office-3.txt "Tarifni broj 1 ... 430 dinara", "Tarifni broj 9 ... 740 dinara" (from 1 July 2026). The e-filing 50 % reduction in Тарифни број 107 is attached to the national application item, not to the international one, so no electronic rate was missed. The eGuide (valid from 13 July 2026) has the lower 10,320 / 1,010 / 20,710 / 14,470; the record explains the choice.

### RU (PCT-RU.ipf, RUB): 5 values, all OK
- OFF_BasicNationalFee 3300 → 4000: OK. RU-office-2.txt "4000 + 1000 за каждый пункт формулы изобретения свыше 10"; RU.txt Annex "Filing fee 4,000 RUB".
- OFF_ClaimFee 700 → 1000: OK. Same quote; RU.txt "claims fee for each claim in excess of ten 1,000 RUB".
- OFF_ClaimFee condition: 0.7 online multiplier removed: OK. RU-office-4.txt "будет отменена скидка в размере 30% на подачу заявления в электронной форме" (Decree 1278 of 18.09.2024, in force 5 October).
- OFF_ExaminationFee base 12500 → 14000 and per independent claim > 1 9200 → 10000: OK. RU-office-2 item 1.8 "14000 + 10000 за каждый независимый пункт формулы свыше 1"; RU.txt "14,000 RUB", "10,000 RUB".

### SE (PCT-SE.ipf, SEK): 1 value, OK
- OFF_Renewal3 1500 → 1600: OK. SE.txt l.381 "... second anniversary (24 months) ... 1,600 SEK"; SE-office-2.txt "N-3 / 3rd annual fee / Yes / 1 600 kr" (regular fee; 1 920 kr is the increased fee, correctly not used).

### SG (PCT-SG.ipf, SGD): 4 values, all OK
- OFF_BasicNationalFee 200 → 210: OK. SG-office.txt "PF37 / Entry into National Phase / S$210"; SG.txt "Patents Form 37 — Entry into national phase 210 SGD".
- OFF_ExaminationFee 1350 → 1420: OK. SG-office "PF12 ... S$1,420 plus S$80 for each claim over 15 claims".
- Per-claim rate 40 → 80 and threshold `> 20` → `> 15`: OK. Same quote; SG.txt "— plus for each claim in excess of 15 claims 80 SGD".

### SK (PCT-SK.ipf, EUR): 3 values, all OK
- OFF_BasicNationalFee 60 → 50: OK (derived, electronic rate). SK.txt "applicant other than the inventor ... 100 EUR" and "reduced by 50% where the application is filed by electronic means, however, the fee shall be reduced by a maximum of 50 EUR"; SK-office-2.txt "216 a) ... 2. iným prihlasovateľom ako pôvodcom ... 100 €". 100 − 50 = 50; the figure "50 EUR" also appears as the cap. Applying the electronic rate follows the protocol.
- OFF_ExaminationFee 116 → 170: OK. SK-office-2 "1. do 10 uplatnených patentových nárokov 170 €"; SK.txt "170 EUR".
- CFee 20 → 30 per claim > 10: OK. SK-office-2 "2. za každý ďalší uplatnený patentový nárok 30 €".

### SV (PCT-SV.ipf, USD): 1 value, OK
- OFF_BasicNationalFee 57.14 → 50: OK. SV.txt "Filing fee / 50 USD". Only source; no office table saved (record says so).

### SY (PCT-SY.ipf, SYP): 3 values, all OK
- OFF_ExaminationFee 10000/1000 → 20000/2000: OK. SY.txt "Examination fee: 20,000 SYP (2,000 SYP)"; parentheses = natural person ("The amount in parentheses is applicable in case of filing by a natural person").
- OFF_Renewal 2nd year 4500/250 → 5500/300: OK. SY.txt "— for the 2nd year: 5,500 SYP (300 SYP)".
- OFF_Renewal 3rd year 5000/300 → 6000/350: OK. SY.txt "— for the 3rd year: 6,000 SYP (350 SYP)".
- Remark: the baseline examination and renewal amounts equal the chapter's utility-model lines (10,000/1,000; 4,500/250; 5,000/300), so the baseline had probably read the wrong block. The new values are taken from the patent block. Correct.

### TM (PCT-TM.ipf, USD): 1 value, OK
- OFF_ClaimFee 25 → 30 per independent claim > 1: OK. TM.txt "Claim fee for each independent claim in excess of one ... 30 USD".

### TR (PCT-TR.ipf, TRY): 3 values, all OK
- OFF_BasicNationalFee 2015 → 16890: OK. TR.txt "Filing fee — for patents 16,890 TRY" (comma = thousands separator, read correctly).
- OFF_SearchFee 805/400 → 5180/2530: OK. TR.txt Annex "Fee for requesting search report (i) Up to 10 applications for natural persons, up to 100 applications for universities 2,530 TRY (ii) Applications which are not under the scope of (i) 5,180 TRY". Not swapped.
- OFF_Renewal3 400 → 3800: OK. TR.txt l.328–330 "Renewal fee for the 3rd year — for patents 3,800 TRY". (3,800 is also the grant-certificate fee, but the renewal line carries it too.)

### US (PCT-US.ipf, USD large/small/micro): 11 values, 9 OK, 2 discrepancies
All amounts from US-office.txt (fee schedule effective 19 January 2025):
- Basic 320/128/64 → 350/140/70: OK. "1631/2631/3631 | 1.492(a) | Basic national stage fee | 350.00 | 140.00 | 70.00".
- Search, US was ISA, 140/56/28 → 150/60/30: OK. "1641 ... U.S. was the ISA | 150.00 | 60.00 | 30.00".
- Search, report by other ISA, 540/216/108 → 580/232/116: OK. "1642 ... search report prepared and provided to USPTO | 580.00 | 232.00 | 116.00".
- Search, all other, 700/280/140 → 770/308/154: OK. "1632 ... all other situations | 770.00 | 308.00 | 154.00".
- Examination 800/320/160 → 880/352/176: OK. "1633 ... all other situations | 880.00 | 352.00 | 176.00".
- Size fee 420/168/84 → 450/180/90 per 50 sheets > 100: OK. "1681 ... each additional 50 sheets that exceeds 100 sheets | 450.00 | 180.00 | 90.00".
- Independent claims 480/192/96 → 600/240/120 per claim > 3: OK. "1614 ... in excess of three | 600.00 | 240.00 | 120.00".
- Claims 100/40/20 → 200/80/40 per claim > 20: OK. "1615 ... in excess of 20 | 200.00 | 80.00 | 40.00".
- Multiple dependent 860/344/172 → 925/370/185: OK. "1616 ... Multiple dependent claim | 925.00 | 370.00 | 185.00".
- Search CASE condition `ISA EQ ISA_US` → `ISA EQ ISA_US AND ISR_AVAILABLE EQ TRUE AND IPRP NEQ IPRP_US`: consistent with 1.492(b)(1)/(b)(2), but contradicts the record (D3).
- Search CASE condition `ISR_AVAILABLE EQ FALSE` → `... AND IPRP NEQ IPRP_US`: same (D3).

## 2. Thousands separators, currencies, electronic/paper, reduced/full

No misread found. Space separators (EP "1 595", NO "6 050", SE "1 600 kr"), comma separators (TR
"16,890", SY "20,000") and the Portuguese decimal comma (PT "64,21 €") were all converted
correctly. Electronic rates are used where the office distinguishes them (EP 135 online, PT 64.21
online, SK 50 electronic; unchanged MA 750/250 and UA 1,280/640 online). Reduced and full rates
are not swapped in RS, SY, TR, QA, NO or US. The one currency problem is NI (D2).

## 3. MATCH spot-checks (one or more per office; baseline = schedule = source)

All OK:
- EA: 2nd-independent-claim examination fee 40,000 (EA-office "дополнительно 40000 рублей").
- EP: filing fee 135 (EP-office-2 "is filed online / 135"); extension BA 102 (EP.txt "Bosnia and Herzegovina ... 102 EUR"); page fee 17 (EP-office-2 "plus EUR 17 for the 36th").
- LA 200,000 (LA.txt "200,000 KIP"); LK 6,900/2,875 (LK.txt "Natural persons 2,875 LKR Others 6,900 LKR"); LR 400 and 80 (LR.txt "400 USD", "80 USD"); LS 250/100 (LS.txt "250 LSL (100 LSL)"); LU 40 and 33 (LU.txt "40 EUR", "33 EUR"); MA 750/250 online and 400/160 (MA.txt "1,000 MAD (750 MAD) 500 MAD ... (250 MAD)", parentheses = online; "per claim 400 MAD per claim 160 MAD"); MD 400 and 10 (MD.txt "400 EUR", "10 EUR"); MG 180,000 and 125,000 (MG.txt); MK 800 (MK.txt "800 MKD").
- MX 3,147 and 61 (MX-office "$3,147.00", "$61.00"); MZ 8,100 and 1,725 (MZ-office "8,100.00", "2.' 1,725.00"); NG 25,000 (NG.txt); NZ 250 (NZ-office "$250"); OA 225,000 filing and 365,000 publication (OA-office); OM 300 (OM.txt "300 OMR (200 OMR)"); PA 128.50 (PA.txt).
- PE 720 and 23.43 (PE.txt); PG 1,000 and 170 (PG.txt); PH 4,320/2,000 (PH.txt "4,320 PHP (2,000 PHP)"); PL 550 (PL.txt); QA renewals 400/200, 500/250 (QA.txt); RO 30 and 500 (RO.txt); RU examination −50 %/−10 % (RU.txt "50% ... (Rospatent)", "10% ... other International Searching Authorities").
- SA 800/400 (SA.txt "800 SAR (400 SAR)"); SC 3,000 (SC.txt); SD 50 (SD.txt); SE 500, 2,500, 150 (SE.txt); SY 5,000/250 filing, 50/10 page, 500/250 claim (SY.txt); TH 500 (TH.txt); TJ 410 (TJ.txt); TM 100 (TM.txt); TN 140 (TN.txt); TT 2,000 and 1,500 (TT.txt); TZ 12,000 (TZ.txt).
- UA 1,280 and 640 online (UA.txt "online 1,280 UAH ... on paper 1,600"); UG 150 (UG.txt "60,000 UGX 150 USD"); UZ 420 and 84 (UZ.txt); VC 1,000 (VC.txt); VN 600,000 priority (VN.txt "per priority 600,000 VND"); WS 500 (WS.txt "500 SAT"); ZA 590 and 130 (ZA.txt); ZM 115 (ZM.txt).
- US: the zero search and examination fees (US-office "1.492(b)(1) ... 0.00", "1.492(c)(1) ... 0.00").
- No MATCH row to check (only UNVERIFIED rows, schedule unchanged): MN, MW, NA, ZW. The ZW chapter figure "6,000 ZWD" equals the schedule's 6000, but the record leaves it UNVERIFIED.

## Discrepancies

**D1. EP: the record contradicts the schedule on the 3rd-year renewal threshold.**
- File: `PCT-EP.ipf`, line 243 (OFF_Renewal3): `YIELD RenFee IF RenMonth GTE 24 #due at the second anniversary of the international filing date`.
- Record: `batch-10.md`, EP notes: "OFF_Renewal3 condition left unchanged at `RenMonth GTE 18`"; the table row reads "≥ 18 months from filing date".
- Source: EP.txt l.410 "This fee is due before the expiration of the month containing the second anniversary (24 months) of the international filing date". The schedule's 24 is supported; the record is stale. The comment two lines above (l.239, "# need to check if 3rd annuity is due (>1.5 years after filing date)") still says 18 months.
- Fix: update the record row, its notes and the stale comment, or revert the threshold. 24 matches the source.

**D2. NI: the currency and the USD amount are not in any saved source, and the record was not updated.**
- File: `PCT-NI.ipf`, line 7 `# Currency: USD`, line 154 `# Fees are set in Central American pesos (CA$, at par with the US dollar) and paid in cordobas.`, line 159 `LET Fee1 AS 240 #first 20 claims`, line 200 `RETURN Currency AS 'USD'`; version `'2026.10' EFFECTIVE 2020-04-03`.
- Source: NI-office.txt "Por las primeras 20 reivindicaciones $CA240.00" and "El monto determinado en Pesos Centroamericanos se cancelará en moneda nacional, aplicando la tasa de cambio que el Banco Central de Nicaragua fijare a la fecha de la transacción". No saved source states that CA$ is at par with USD (grep for "dólar", "dolar", "paridad", "equivalen" finds nothing). The fee is payable in córdobas. NI.txt (eGuide, valid from 1 January 2026, the more recent document) still says "Filing fee / 200 NIO". Under the protocol, the more recent source would win unless its staleness is documented. The record argues that it is stale.
- Record: `batch-07.md` NI still says "Version line: unchanged ('2024.1' EFFECTIVE 2024-01-01); no row verified" and gives the row as UNVERIFIED. Its summary row (NI: Unverified 1, "unchanged (2024-01-01)") no longer describes the schedule.
- Fix: save a source for the CA$ = USD parity (for example a SIECA or Banco Central de Nicaragua page), or record the parity as the lead's assumption. Then update the NI record section and summary row to UPDATED (amount, currency), with effective date 3 April 2020. The claim fee over 20 (CA$20) stays MISSING.

**D3. US: the record contradicts the schedule on the search-fee conditions.**
- File: `PCT-US.ipf`, lines 173 and 185 (OFF_SearchFee): `CASE ISA EQ ISA_US AND ISR_AVAILABLE EQ TRUE AND IPRP NEQ IPRP_US AS` (was `CASE ISA EQ ISA_US AS`) and `CASE ISR_AVAILABLE EQ FALSE AND IPRP NEQ IPRP_US AS` (was `CASE ISR_AVAILABLE EQ FALSE AS`).
- Record: `batch-10.md`, US notes: "Baseline conditions not changed ... `OFF_SearchFee` sums its CASE blocks, so with IPRP = US and ISA = US it yields 0 + 150, not 0". The verdict column has no UPDATED (condition) row.
- Source: US-office.txt "1.492(b)(1) | National stage search fee - U.S. was the ISA or IPEA and all claims satisfy PCT Article 33(1)-(4) | 0.00"; "1.492(b)(2) ... U.S. was the ISA | 150.00"; "1.492(b)(4) ... all other situations | 770.00". The new conditions make the four cases exclusive and fix the double charge the record describes, so they agree with the source. Only the record is stale.
- Fix: add an UPDATED (condition) row (or a note) to the US section and correct the summary counts.

Minor points (not counted as discrepancies):
- TM: `batch-09.md` says the `VERIFY MONOTONIC FEE OFF_ClaimFee WITH RESPECT TO ClaimCount` doubt was "not changed". The schedule now verifies against `IndependentClaimCount`. This is out of scope here (VERIFY change), but the record is stale.
- ZW: the only row is UNVERIFIED, yet the version line moved to `'2026.10' EFFECTIVE 2026-01-01` with a "Checked against" comment. MN, MW, NA and NI (as recorded) keep the version unchanged when no row is verified. The practice is inconsistent; the record does describe the change it made.
- EP: SF2 (−1,365) applies only to international applications filed up to 31 March 2028 (EP-office-2 Art. 4). The schedule has no date limit; the record notes this.
- PT: 64.21 (office table, PCT-entry item) against 128.42 (eGuide, national filing fee). Both take effect on 1 July 2026; the record leaves this to the lead.
- EA: the 25 %/40 % ISR reductions also apply to the claim fee (EA-office "размеры единой процедурной пошлины и пошлины, установленной в абзаце третьем правила 24(7)"). The schedule reduces only the unitary fee; the record lists this as MISSING.

## Addendum: MN, MW and NA as settled by the lead

Checked on 20 October 2026 by a further assistant that made none of the edits, against the
sources saved for the settlement (`sources/MN-office*.txt`, `MW-office.txt`, `NA-office.txt`,
`NA-office-4.txt`) and the three schedules. Four values, all found with the meaning the schedule
gives them:

- MN filing fee, up to five claims: 35,000 MNT. OK. MN-office "1-5 томъёололтой 35000".
- MN filing fee, each claim above five: 5,000 MNT. OK, with a note: the cell reads
  "+Томъёоллын тоо *5000" ("+ number of claims × 5000") in the row "-5-аас дээш томъёолол тус
  бүрд" ("for each claim above five"); the row label carries the excess-claims reading that
  `YIELD 5000*(ClaimCount-5) IF ClaimCount GT 5` encodes. EFFECTIVE 2021-07-01: OK, MN-office-2
  "2021 оны 7 дугаар сарын 1-ний өдрөөс эхлэн дагаж мөрдсүгэй" (Resolution No. 151 of 19 May 2021).
  The fee page's link to the PDF is not in the saved text, whose markup was stripped; the PDF's
  URL heads MN-office.txt.
- MW complete specification: 20,000 MWK (US$100 foreign). OK. MW-office "Government Notice No.
  45", "( b ) complete . . . . . ' 20,000.00 100.00"; gazette of 10th August, 2020.
- NA application fee: 1,000 NAD. OK. NA-office Part B "1. Application fee / Patent 1000", in effect
  1 August 2018; NA-office-4 "Patent application fees – N$ 1000".

The NI record (D2 above) now reads UPDATED (amount, currency) with effective date 3 April 2020.
