<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <body>
        <h4>Hi All,</h4>
        <div>
          <h4>
            Please find the status of the files not created before - <xsl:value-of select="FileCheckError/RunDate"/>
          </h4>
        </div>
        <h4>
          <u>Summary</u>
        </h4>
        <div>
          <h4>
            Number of Files not Created - <xsl:value-of select="FileCheckError/FilesCount"/>
          </h4>
        </div>
        <h4>
          <u>
            Batch Issues - 
          </u>
        </h4>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>File Name</th>
            <th>Issue</th>
            <th>Scheduled Creation Date</th>
          </tr>
          <xsl:for-each select="FileCheckError/FileObjects/FileObject">
            <tr>
              <td>
                <xsl:value-of select="FileName"/>
              </td>
              <td>
                <xsl:value-of select="Issue"/>
              </td>
              <td>
                <xsl:value-of select="Date"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>