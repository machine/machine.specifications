<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" />

  <xsl:variable name="mspec.root" select="//MSpec"/>

  <xsl:template match="/">
    <xsl:param name="totalSpecs" select="count($mspec.root//specification)" />
    <xsl:param name="passedSpecs" select="count($mspec.root//specification[@status='passed'])" />
    <xsl:param name="failedSpecs" select="count($mspec.root//specification[@status='failed'])" />
    <xsl:param name="ignoredSpecs" select="count($mspec.root//specification[@status='ignored'])" />
    <xsl:param name="notImplementedSpecs" select="count($mspec.root//specification[@status='not-implemented'])" />

    <xsl:param name="barWidth" select="300" />
    <xsl:param name="passedSpecsLength" select="round($passedSpecs div $totalSpecs * $barWidth)" />
    <xsl:param name="failedSpecsLength" select="round($failedSpecs div $totalSpecs * $barWidth)" />
    <xsl:param name="ignoredSpecsLength" select="round($ignoredSpecs div $totalSpecs * $barWidth)" />
    <xsl:param name="notImplementedSpecsLength" select="round($notImplementedSpecs div $totalSpecs * $barWidth)" />

    <style type="text/css">
      #mspec .passed { background-color: #80FF00 }
      #mspec .failed { background-color: #FA5858 }
      #mspec .ignored { background-color: #FAAC58 }
      #mspec .not-implemented { background-color: #f0f }
      #mspec .float-l { float:left; }
      #mspec .bar { height:17px;vertical-alignment:middle }
    </style>

    <table id="mspec" class="section-table"  cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td class="sectionheader" colspan="5">
          MSpec Summary
        </td>
      </tr>
      <tr>
        <td width="300px">
          <div>
            <div class="float-l" style="position:absolute">Specs ran:</div>
            <div class="passed float-l bar"><xsl:attribute name="style">width:<xsl:value-of select="$passedSpecsLength"/>px;left:0px;</xsl:attribute>
              </div>
            <div class="failed float-l bar">
                <xsl:attribute name="style">width:<xsl:value-of select="$failedSpecsLength"/>px;left:<xsl:value-of select="$passedSpecsLength"/>px;</xsl:attribute>
              </div>
            <div class="ignored float-l bar">
                <xsl:attribute name="style">width:<xsl:value-of select="$ignoredSpecsLength"/>px;left:<xsl:value-of select="$passedSpecsLength + $failedSpecsLength"/>px;</xsl:attribute>
              </div>
            <div class="not-implemented float-l bar">
                <xsl:attribute name="style">width:<xsl:value-of select="$notImplementedSpecsLength"/>px;left:<xsl:value-of select="$passedSpecsLength + $failedSpecsLength + $ignoredSpecsLength"/>px;</xsl:attribute>
              </div>
          </div>
        </td>
        <td >
          <xsl:value-of select="$totalSpecs"/>
        </td>
      </tr>
      <tr>
        <td>
          <p class="passed bar">Passed:</p>
        </td>
        <td>
          <xsl:value-of select="$passedSpecs"/>
        </td>
      </tr>
      <tr>
        <td>
          <p class="failed bar">Failed:</p>
        </td>
        <td>
          <xsl:value-of select="$failedSpecs"/>
        </td>
      </tr>
      <tr>
        <td>
          <p class="ignored bar">Ignored:</p>
        </td>
        <td>
          <xsl:value-of select="$ignoredSpecs"/>
        </td>
      </tr>
      <tr>
        <td>
          <p class="not-implemented bar">Not implemented:</p>
        </td>
        <td>
          <xsl:value-of select="$notImplementedSpecs"/>
        </td>
      </tr>
      <tr>
        <td colspan="2">
          <xsl:choose>
            <xsl:when test="$failedSpecs &gt; 0">
              <strong>Specs failed</strong>
            </xsl:when>
            <xsl:otherwise>
              <strong>All specs passed</strong>
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </tr>
    </table>
    <xsl:apply-templates select="$mspec.root" />
  </xsl:template>
</xsl:stylesheet>
