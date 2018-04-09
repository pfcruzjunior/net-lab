alter table NotaFiscalItem add BaseIpi decimal(18,5) null;
alter table NotaFiscalItem add AliquptaIpi decimal(18,5) null;
alter table NotaFiscalItem add ValorIpi decimal(18,5) null;

GO
ALTER PROCEDURE P_NOTA_FISCAL_ITEM
(
	@pId int,
    @pIdNotaFiscal int,
    @pCfop varchar(5),
    @pTipoIcms varchar(20),
    @pBaseIcms decimal(18,5),
    @pAliquotaIcms decimal(18,5),
    @pValorIcms decimal(18,5),
	@pBaseIpi decimal(18,5),
    @pAliquotaIpi decimal(18,5),
    @pValorIpi decimal(18,5),
    @pNomeProduto varchar(50),
    @pCodigoProduto varchar(20)
)
AS
BEGIN
	IF (@pId = 0)
	BEGIN 		
		INSERT INTO [dbo].[NotaFiscalItem]
           ([IdNotaFiscal]
           ,[Cfop]
           ,[TipoIcms]
           ,[BaseIcms]
           ,[AliquotaIcms]
           ,[ValorIcms]
		   ,[BaseIpi]
		   ,[AliquptaIpi]
		   ,[ValorIpi]
           ,[NomeProduto]
           ,[CodigoProduto])
		VALUES
           (@pIdNotaFiscal,
			@pCfop,
			@pTipoIcms,
			@pBaseIcms,
			@pAliquotaIcms,
			@pValorIcms,
			@pBaseIpi,
			@pAliquotaIpi,
			@pValorIpi,
			@pNomeProduto,
			@pCodigoProduto)

		SET @pId = @@IDENTITY
	END
	ELSE
	BEGIN
		UPDATE [dbo].[NotaFiscalItem]
		SET [IdNotaFiscal] = @pIdNotaFiscal
			,[Cfop] = @pCfop
			,[TipoIcms] = @pTipoIcms
			,[BaseIcms] = @pBaseIcms
			,[AliquotaIcms] = @pAliquotaIcms
			,[ValorIcms] = @pValorIcms
			,[BaseIpi] = @pBaseIpi
			,[AliquptaIpi] = @pAliquotaIpi
			,[ValorIpi] = @pValorIpi
			,[NomeProduto] = @pNomeProduto
			,[CodigoProduto] = @pCodigoProduto
		 WHERE Id = @pId
	END	    
END

select * from NotaFiscalItem

GO
CREATE PROCEDURE P_NOTA_FISCAL_ITEM_SELECT
AS 
BEGIN
	SELECT
		itemNf.Cfop	'CFOP'
		,SUM(BaseIcms) 'Valor Total da Base de ICMS'
		,SUM(ValorIcms) 'Valor Total do ICMS'
		,SUM(BaseIpi) 'Valor Total da Base de IPI'
		,SUM(ValorIpi) 'Valor Total do IPI'
	FROM dbo.NotaFiscalItem itemNf
	GROUP BY itemNf.Cfop
END