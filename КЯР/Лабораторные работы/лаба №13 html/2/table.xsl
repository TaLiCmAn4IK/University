<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <head>
                <title>xslt</title>
            </head>
            <body>
                <h2 align="center">Аттестация</h2>
                <table align="center" border="1">
                    <tr>
                        <th>Студент</th>
                        <th>Оценка</th>
                    </tr>
                    <xsl:for-each select="main/rw">
                        <xsl:sort select="user" lang="ru"/>
                        <tr>
                            <td><xsl:value-of select="user"/></td>
                            <xsl:choose>
                                <xsl:when test="point &gt; 8">
                                    <td align="center" bgcolor="#00FF00"><xsl:value-of select="point"/></td>
                                </xsl:when>
                                <xsl:when test="point &gt; 3">
                                    <td align="center" bgcolor="#FFFFFF"><xsl:value-of select="point"/></td>
                                </xsl:when>
                                <xsl:otherwise>
                                    <td align="center" bgcolor="#FF0000"><xsl:value-of select="point"/></td>
                                </xsl:otherwise>
                            </xsl:choose>
                        </tr>
                    </xsl:for-each>
                </table>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>