DEFINE LIST EntityType AS 'Select the desired entity type'
VALUE 'Normal' AS NormalEntity
VALUE 'Small' AS SmallEntity
VALUE 'Micro' AS MicroEntity
DEFAULT NormalEntity
END

DEFINE LIST SituationType AS 'Situation type'
VALUE 'IRRP (Chapter II) prepared by the IPEA/US' AS PreparedIPEA
VALUE 'International search fee paid as ISA' AS PaidAsISA
VALUE 'Search report prepared by an ISA other than the US' AS PreparedISA
VALUE 'All other situations' AS AllOtherSituations
DEFAULT PreparedIPEA
END

DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
BETWEEN 10 AND 1000
DEFAULT 15
END

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 0 AND 1000
DEFAULT 0
END

DEFINE BOOLEAN ContainsDependentClaims AS 'Does this contain dependent claims?'
DEFAULT TRUE
END

COMPUTE FEE BasicNationalFee
CASE EntityType IS NormalEntity YIELD 320
CASE EntityType IS SmallEntity YIELD 128
CASE EntityType IS MicroEntity YIELD 64
END

COMPUTE FEE SearchFee
CASE SituationType IS PreparedIPEA AND EntityType IS NormalEntity YIELD 0
CASE SituationType IS PreparedIPEA AND EntityType IS SmallEntity YIELD 0
CASE SituationType IS PreparedIPEA AND EntityType IS MicroEntity YIELD 0
CASE SituationType IS PaidAsISA AND EntityType IS NormalEntity YIELD 140
CASE SituationType IS PaidAsISA AND EntityType IS SmallEntity YIELD 56
CASE SituationType IS PaidAsISA AND EntityType IS MicroEntity YIELD 28
CASE SituationType IS PreparedISA AND EntityType IS NormalEntity YIELD 540
CASE SituationType IS PreparedISA AND EntityType IS SmallEntity YIELD 216
CASE SituationType IS PreparedISA AND EntityType IS MicroEntity YIELD 108
CASE EntityType IS NormalEntity YIELD 700
CASE EntityType IS SmallEntity YIELD 280
CASE EntityType IS MicroEntity YIELD 140
END

COMPUTE FEE SearchFeeAlternate
CASE SituationType IS PreparedIPEA AS
	CASE EntityType IS NormalEntity YIELD 0
	CASE EntityType IS SmallEntity YIELD 0
	CASE EntityType IS MicroEntity YIELD 0
ENDCASE
CASE SituationType IS PaidAsISA AS
	CASE EntityType IS NormalEntity YIELD 140
	CASE EntityType IS SmallEntity YIELD 56
	CASE EntityType IS MicroEntity YIELD 28
ENDCASE
CASE SituationType IS PreparedISA AS
	CASE EntityType IS NormalEntity YIELD 540
	CASE EntityType IS SmallEntity YIELD 216
	CASE EntityType IS MicroEntity YIELD 108
ENDCASE
CASE EntityType IS NormalEntity YIELD 700
CASE EntityType IS SmallEntity YIELD 280
CASE EntityType IS MicroEntity YIELD 140
END

COMPUTE FEE ExaminationFee
CASE SituationType IS PreparedIPEA AS
	CASE EntityType IS NormalEntity YIELD 0
	CASE EntityType IS SmallEntity YIELD 0
	CASE EntityType IS MicroEntity YIELD 0
ENDCASE
CASE EntityType IS NormalEntity YIELD 800
CASE EntityType IS SmallEntity YIELD 320
CASE EntityType IS MicroEntity YIELD 160
END

COMPUTE FEE SheetFee
CASE SheetCount OVER 100 AND EntityType IS NormalEntity YIELD 420 * SheetCount / 50
CASE SheetCount OVER 100 AND EntityType IS SmallEntity YIELD 168 * SheetCount / 50
CASE SheetCount OVER 100 AND EntityType IS MicroEntity YIELD 84 * SheetCount / 50
END

COMPUTE FEE SheetFeeAlternate
CASE SheetCount OVER 100 AND EntityType IS NormalEntity YIELD 420 * SheetCount / 50
CASE SheetCount OVER 100 AND EntityType IS SmallEntity YIELD 168 * SheetCount / 50
CASE SheetCount OVER 100 AND EntityType IS MicroEntity YIELD 84 * SheetCount / 50
END

COMPUTE FEE ClaimFee
CASE ClaimCount OVER 3 AND EntityType IS NormalEntity YIELD 480 * ClaimCount
CASE ClaimCount OVER 3 AND EntityType IS SmallEntity YIELD 192
CASE ClaimCount OVER 3 AND EntityType IS MicroEntity YIELD 96
END

COMPUTE FEE DependentClaimFee
CASE ContainsDependentClaims IS TRUE AND EntityType IS NormalEntity YIELD 860
CASE ContainsDependentClaims IS TRUE AND EntityType IS SmallEntity YIELD 344
CASE ContainsDependentClaims IS TRUE AND EntityType IS MicroEntity YIELD 172
END