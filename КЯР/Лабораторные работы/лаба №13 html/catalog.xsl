<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <title>Каталог товаров</title>
        <link rel="stylesheet" type="text/css" href="styleXML.css"/>
      </head>
      <body>
        <h1>Каталог товаров</h1>
        <table>
          <tr>
            <th>ID</th>
            <th>Название</th>
            <th>Цена</th>
            <th>Описание</th>
          </tr>
          <xsl:for-each select="catalog/product">
            <xsl:sort select="substring-before(price, '$')" data-type="number" order="ascending"/>
            <tr>
              <td><xsl:value-of select="id"/></td>
              <td><xsl:value-of select="name"/></td>
              <td><xsl:value-of select="price"/></td>
              <td><xsl:value-of select="description"/></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>