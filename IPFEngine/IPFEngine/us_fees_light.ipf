COMPUTE FEE BasicNationalFee
YIELD 320 IF EntityType IS NormalEntity
YIELD 128 IF EntityType IS SmallEntity 
YIELD 64 IF EntityType IS MicroEntity
ENDCOMPUTE

COMPUTE FEE SearchFee
CASE SituationType IS PreparedIPEA AS
	YIELD 0 IF EntityType IS NormalEntity
	YIELD 0 IF EntityType IS SmallEntity
	YIELD 0 IF EntityType IS MicroEntity
ENDCASE
CASE SituationType IS PaidAsISA AS
	YIELD 140 IF EntityType IS NormalEntity
	YIELD 56 IF EntityType IS SmallEntity
	YIELD 28 IF EntityType IS MicroEntity
ENDCASE
CASE SituationType IS PreparedISA AS
	YIELD 540 IF EntityType IS NormalEntity
	YIELD 216 IF EntityType IS SmallEntity
	YIELD 108 IF EntityType IS MicroEntity
ENDCASE
YIELD 700 IF EntityType IS NormalEntity
YIELD 280 IF EntityType IS SmallEntity
YIELD 140 IF EntityType IS MicroEntity
ENDCOMPUTE
