<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>

  <xsl:variable name="mspec.root" select="//MSpec"/>

  <xsl:template match="/">
    <div id="mspec-report">
      <style type="text/css">
        .passed { background-color: #80FF00 }
        .failed { background-color: #FA5858 }
        .ignored { background-color: #FAAC58 }
        .not-implemented { background-color: #f0f }
        .summary { width: 250px }
        .totals { color:LightGrey; }
      </style>
      <xsl:apply-templates select="//MSpec" />
    </div>
  </xsl:template>

  <xsl:template match="assembly">
    <h2>
      <xsl:value-of select="@name"/>
    </h2>

    <h3 class="totals">
      <xsl:value-of select="count(concern)"/> concern<xsl:if test="count(concern) != 1">s</xsl:if>, <xsl:value-of select="count(concern/context)"/> context<xsl:if test="count(concern/context) != 1">s</xsl:if>, <xsl:value-of select="count(concern/context/specification)"/> specification<xsl:if test="count(concern/context/specification) != 1">s</xsl:if>
    </h3>

    <br/>
    <xsl:apply-templates select="concern"/>
      
    <hr/>    
  </xsl:template>

  <xsl:template match="concern">
    <h3>
      <xsl:value-of select="@name"/>
    </h3>
    <p class="totals">
      <strong>
        <xsl:value-of select="count(context)"/> context<xsl:if test="count(context) != 1">s</xsl:if>, <xsl:value-of select="count(context/specification)"/> specification<xsl:if test="count(context/specification) != 1">s</xsl:if>
      </strong>
    </p>
    <xsl:apply-templates select="context"/>
      
    <hr/>
  </xsl:template>

  <xsl:template match="context">
    <strong>
      <xsl:value-of select="@name"/>
    </strong>
    <p class="totals">
      <xsl:value-of select="count(specification)"/> specification<xsl:if test="count(specification) != 1">s</xsl:if>
    </p>
    <ul>
      <xsl:apply-templates select="specification"/>        
    </ul>
    <br/>    
  </xsl:template>

  <xsl:template match="specification">
    <li>
      <span>
        <xsl:attribute name="class">
          <xsl:value-of select="@status"/>
        </xsl:attribute>
        <xsl:value-of select="@name"/>
        <xsl:if test="@timeTaken">
          - <xsl:value-of select="@timeTaken"/> seconds        
        </xsl:if>
      </span>
    </li>    
  </xsl:template>

  <xsl:template match="//MSpec">

    <h1>MSpec Report</h1>
    <br/>
    <span class="totals">
      <i>Generated on <xsl:value-of select="generated/date"/> at <xsl:value-of select="generated/time"/></i>
    </span>
    <div>
      <p>
        Total specifications: <xsl:value-of select="count($mspec.root//specification)"/>
      </p>
      <p class="passed summary">
        Passed specifications: <xsl:value-of select="count($mspec.root//specification[@status='passed'])"/>
      </p>
      <p class="failed summary">
        Failed specifications: <xsl:value-of select="count($mspec.root//specification[@status='failed'])"/>
      </p>
      <p class="ignored summary">
        Ignored specifications: <xsl:value-of select="count($mspec.root//specification[@status='ignored'])"/>
      </p>
      <p class="not-implemented summary">
        Not implemented specifications: <xsl:value-of select="count($mspec.root//specification[@status='not-implemented'])"/>
      </p>
    </div>
    <hr/>

    <xsl:apply-templates select="assembly"/>

  </xsl:template>
</xsl:stylesheet>
