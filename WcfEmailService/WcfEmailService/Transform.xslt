<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <body>
        <h2>Benefit Management Validation Bot</h2>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>FieldName</th>
            <th>Validation</th>
          </tr>
          <xsl:for-each select="DocValidation/Records/Record">
            <tr>
              <td>
                <xsl:value-of select="fieldName"/>
              </td>
              <td>
                <xsl:value-of select="validation"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>