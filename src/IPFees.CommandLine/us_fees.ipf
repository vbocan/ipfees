DEFINE LIST EntityType AS 'Select the desired entity type'
CHOICE NormalEntity AS 'Normal'
CHOICE SmallEntity AS 'Small'
CHOICE MicroEntity AS 'Micro'
DEFAULT NormalEntity
ENDDEFINE

DEFINE LIST SituationType AS 'Situation type'
CHOICE PreparedIPEA AS 'IRRP (Chapter II) prepared by the IPEA/US'
CHOICE PaidAsISA AS 'International search fee paid as ISA'
CHOICE PreparedISA AS 'Search report prepared by an ISA other than the US'
CHOICE AllOtherSituations AS 'All other situations'
DEFAULT PreparedIPEA
ENDDEFINE

DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
BETWEEN 10 AND 1000
DEFAULT 15
ENDDEFINE

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 0 AND 1000
DEFAULT 0
ENDDEFINE

DEFINE BOOLEAN ContainsDependentClaims AS 'Does this contain dependent claims?'
DEFAULT TRUE
ENDDEFINE

COMPUTE FEE BasicNationalFee
YIELD 320 IF EntityType EQ NormalEntity
YIELD 128 IF EntityType EQ SmallEntity 
YIELD 64 IF EntityType EQ MicroEntity
ENDCOMPUTE

COMPUTE FEE SearchFee
CASE SituationType EQ PreparedIPEA AS
	YIELD 0 IF EntityType EQ NormalEntity
	YIELD 0 IF EntityType EQ SmallEntity
	YIELD 0 IF EntityType EQ MicroEntity
ENDCASE
CASE SituationType EQ PaidAsISA AS
	YIELD 140 IF EntityType EQ NormalEntity
	YIELD 56 IF EntityType EQ SmallEntity
	YIELD 28 IF EntityType EQ MicroEntity
ENDCASE
CASE SituationType EQ PreparedISA AS
	YIELD 540 IF EntityType EQ NormalEntity
	YIELD 216 IF EntityType EQ SmallEntity
	YIELD 108 IF EntityType EQ MicroEntity
ENDCASE
YIELD 700 IF EntityType EQ NormalEntity
YIELD 280 IF EntityType EQ SmallEntity
YIELD 140 IF EntityType EQ MicroEntity
ENDCOMPUTE

COMPUTE FEE ExaminationFee
CASE SituationType EQ PreparedIPEA AS
	YIELD 0 IF EntityType EQ NormalEntity
	YIELD 0 IF EntityType EQ SmallEntity
	YIELD 0 IF EntityType EQ MicroEntity
ENDCASE
YIELD 800 IF EntityType EQ NormalEntity
YIELD 360 IF EntityType EQ SmallEntity
YIELD 160 IF EntityType EQ MicroEntity
ENDCOMPUTE

COMPUTE FEE SheetFee
LET F AS 420
YIELD F*((SheetCount-100)/50) IF SheetCount GT 100 AND EntityType EQ NormalEntity
YIELD F/2*((SheetCount-100)/50) IF SheetCount GT 100 AND EntityType EQ SmallEntity
YIELD F/3*((SheetCount-100)/50) IF SheetCount GT 100 AND EntityType EQ MicroEntity
ENDCOMPUTE

COMPUTE FEE ClaimFee
LET F AS 480
LET G AS 192
LET H AS 96
YIELD (F*ClaimCount) IF ClaimCount GT 3 AND EntityType EQ NormalEntity
YIELD (G*ClaimCount) IF ClaimCount GT 3 AND EntityType EQ SmallEntity
YIELD (H*ClaimCount) IF ClaimCount GT 3 AND EntityType EQ MicroEntity
ENDCOMPUTE