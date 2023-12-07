<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="Publications">
		<html>
			<body>
				<table border="1">
					<tr>
						<th>Name</th>
						<th>Faculty</th>
						<th>Department</th>
						<th>Data</th>
					</tr>
					<xsl:apply-templates/>
				</table>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="publications">
		<tr>
			<td>
				<xsl:value-of select="@NAME"/>
			</td>
			<td>
				<xsl:value-of select="@FACULTY"/>
			</td>
			<td>
				<xsl:value-of select="@DEPARTMENT"/>
			</td>
			<td>
				<xsl:value-of select="@DATA"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
