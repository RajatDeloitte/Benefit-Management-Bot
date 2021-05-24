<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <body>
        <h4>Hi All,</h4>
        <div>
          <h4>
            Count of records processed - <xsl:value-of select="ValidationObject/totalRecords"/>
          </h4>
        </div>
        <div>
          <h4>
            Count of records failed - <xsl:value-of select="ValidationObject/failedRecords"/>
          </h4>
        </div>
        <h4>
          <u>Summary</u>
        </h4>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Line Number</th>
            <th>Identifier</th>
            <th>Error Message</th>
            <th>Record Type</th>
            <th>Record Value</th>
          </tr>
          <xsl:for-each select="ValidationObject/erorrecords/EmailErrorRecord">
            <tr>
              <td>
                <xsl:value-of select="lineNumber"/>
              </td>
              <td>
                <xsl:value-of select="identifier"/>
              </td>
              <td>
                <xsl:value-of select="validationMessage"/>
              </td>
              <td>
                <xsl:value-of select="recordType"/>
              </td>
              <td>
                <xsl:value-of select="recordValue"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>