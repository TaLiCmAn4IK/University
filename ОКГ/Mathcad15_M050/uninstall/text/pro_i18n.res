! This is 'pro_i18n.res' file.
! It is used for Internationalization of
! Pro family of products such as Pro/Engineer.
!
! This file will only ever be found in the $PRO_DIRECTORY/text directory.
!
! The commented lines are already hardcoded into the application. In general,
! new languages should be hardcoded, then commented here; add new lines without
! a comment prefix (!) to extend the list of language+locale pairs recognized
! at a customer site.
!--------------------------------------------------------------------------
! Change History
! 26-Aug-02 J-03-32 ECW       $$1 Remove Hitachi, NEC, IBM, Alpha sections. Update Linux, Solaris.
! 26-Nov-02 J-03-39 ECW       $$2 Add @euro and iso885915 locales.
! 20-Jun-03 K-01-09 TGH       $$3 Removed zh_TW.EUC (SPR 1024516)
! 10-May-04 K-03-01 TWH       $$4 Add win32 system regional locale settings
! 12-May-04 K-03-01 PMORK     $$5 Added Eastern European locales
! 26-May-05 K-03-24 JSP       $$6 Added en_US.UTF-8 for Linux
! 13-Jul-05 K-03-28 AUC/PMORK $$7 Added french, german, italian, spanish locales for Linux
!                                 Cleaned up formating. Added EE languages to Windows
!                                 specific section.
! 26-Jul-05 K-03-29 JSP	      $$8 Remove en_US.UTF-8 for Linux
! 07-Sep-05 K-03-31 PMORK     $$9 Reference to an internal function removed from a comment
! 27-Sep-07 L-01-39 CHI      $$10 Update location of Windows language codes
! 20-Sep-08 L-03-18 CHI      $$11 comment to match lang_envmnt.c
!				  Add info, items to #4
! 05-Jan-09 L-03-23 CHI      $$12 Add Russian support
[LOCALE]

! Do NOT un-comment this sub-section.
! {DEFAULT}
!    USASCII usascii
!    USASCII C
!    JAPANESE japanese
!    FRENCH french
!    GERMAN german
!    RUSSIAN russian
!    RUSSIAN ru
!    ITALIAN italian
!    SPANISH spanish
!    GENERIC_LANG usascii
!    KOREAN korean
!    CHINESE_TW chinese_tw
!    CHINESE_CN chinese_cn
!    GENERIC_LANG_SB usascii
!    HEBREW hebrew
!    GREEK greek
!    TURKISH turkish
!    CZECH czech
!    POLISH polish
!    HUNGARIAN hungarian
!    SLOVENIAN slovenian
!    PORTUGUESE portuguese
!    SLOVAK slovak

  {WINDOWS}
!    CHINESE_TW	cht
!    CHINESE_TW	chinese-traditional
!    CHINESE_CN	chs
!    CHINESE_CN	chinese-simplified
!    HEBREW	hebrew
!    USASCII    english

! Windows specific section
!
! Starting from K-03-01, Pro/ENGINEER on Windows queries the current 
! user locale and attempts to map it to a valid Pro/ENGINEER language. 
! The mapping is based on the language entries listed below.
! 
! This language auto-detect mechanism is disabled if either the LANG 
! or PRO_LANG variable is defined.
! 
! Entry values of this list were based on
!	http://msdn.microsoft.com/library/default.asp?url=/library/en-us/intl/nls_238z.asp
!
! As of 26 Sep 07, this forwards to
!	http://msdn2.microsoft.com/en-us/library/ms776260.aspx
! No predictions how much longer the forward will work.

!    CHINESE_TW	0x0404 # Chinese (Taiwan)			zh-TW
!    CHINESE_CN	0x0804 # Chinese (PRC)				zh-CN
!    CHINESE_CN	0x0c04 # Chinese (Hong Kong SAR, PRC)		zh-HK
!    CHINESE_CN	0x1004 # Chinese (Singapore)			zh-SG
!    CHINESE_CN	0x1404 # Chinese (Macao SAR)			zh-MO
!    FRENCH	0x040c # French (France)			fr-FR
!    FRENCH	0x080c # French (Belgium)			fr-BE
!    FRENCH	0x0c0c # French (Canada)			fr-CA
!    FRENCH	0x100c # French (Switzerland)			fr-CH
!    FRENCH	0x140c # French (Luxembourg)			fr-LU
!    FRENCH	0x180c # French (Monaco)			fr-MC
!    GERMAN	0x0407 # German (Germany)			de-DE
!    GERMAN	0x0c07 # German (Austria)			de-AT
!    GERMAN	0x0807 # German (Switzerland)			de-CH
!    GERMAN	0x1007 # German (Luxembourg)			de-LU
!    GERMAN	0x1407 # German (Liechtenstein)			de-LI
!    HEBREW	0x040d # Hebrew (Israel)			he-IL
!    ITALIAN	0x0410 # Italian (Italy)			it-IT
!    ITALIAN	0x0810 # Italian (Switzerland)			it-CH
!    JAPANESE	0x0411 # Japanese (Japan)			ja-JP
!    KOREAN	0x0412 # Korean (Korea)				ko-KR
!    KOREAN	0x0812 # Korean (Johab; NT 4--; removable?)
!    RUSSIAN	0x0419 # Russian (Russia)			ru-RU
!    SPANISH	0x040a # Spanish (Spain, Traditional Sort)	es-ES_tradnl
!    SPANISH	0x080a # Spanish (Mexico)			es-MX
!    SPANISH	0x0c0a # Spanish (Spain)			es-ES
!    SPANISH	0x100a # Spanish (Guatemala)			es-GT
!    SPANISH	0x140a # Spanish (Costa Rica)			es-CR
!    SPANISH	0x180a # Spanish (Panama)			es-PA
!    SPANISH	0x1c0a # Spanish (Dominican Republic)		es-DO
!    SPANISH	0x200a # Spanish (Venezuela)			es-VE
!    SPANISH	0x240a # Spanish (Colombia)			es-CO
!    SPANISH	0x280a # Spanish (Peru)				es-PE
!    SPANISH	0x2c0a # Spanish (Argentina)			es-AR
!    SPANISH	0x300a # Spanish (Ecuador)			es-EC
!    SPANISH	0x340a # Spanish (Chile)			es-CL
!    SPANISH	0x380a # Spanish (Uruguay)			es-UY
!    SPANISH	0x3c0a # Spanish (Paraguay)			es-PY
!    SPANISH	0x400a # Spanish (Bolivia)			es-BO
!    SPANISH	0x440a # Spanish (El Salvador)			es-SV
!    SPANISH	0x480a # Spanish (Honduras)			es-HN
!    SPANISH	0x4c0a # Spanish (Nicaragua)			es-NI
!    SPANISH	0x500a # Spanish (Puerto Rico)			es-PR
!    SPANISH	0x540a # Spanish (United States; Vista++ only)	es-US
!    GREEK	0x0408 # Greek (Greece)				el-GR
!    TURKISH	0x041f # Turkish (Turkey)			tr-TR
!    CZECH	0x0405 # Czech (Czech Republic)			cs-CZ
!    POLISH	0x0415 # Polish (Poland)			pl-PL
!    HUNGARIAN	0x040e # Hungarian (Hungary)			hu-HU
!    SLOVENIAN	0x0424 # Slovenian (Slovenia)			sl-SI
!    PORTUGUESE	0x0416 # Portuguese (Brazil)			pt-BR
!    PORTUGUESE	0x0816 # Portuguese (Portugal)			pt-PT
!    SLOVAK	0x041b # Slovak (Slovakia)			sk-SK
!    USASCII	0x0c09 # English (Australia)			en-AU
!    USASCII	0x2809 # English (Belize)			en-BZ
!    USASCII	0x1009 # English (Canada)			en-CA
!    USASCII	0x2409 # English (Caribbean)			en-029
!    USASCII	0x4009 # English (India; Vista++ only)		en-IN
!    USASCII	0x1809 # English (Ireland)			en-IE
!    USASCII	0x2009 # English (Jamaica)			en-JM
!    USASCII	0x4409 # English (Malaysia; Vista++ only)	en-MY
!    USASCII	0x1409 # English (New Zealand)			en-NZ
!    USASCII	0x3409 # English (Philippines)			en-PH
!    USASCII	0x4809 # English (Singapore; Vista++ only)	en-SG
!    USASCII	0x1c09 # English (South Africa)			en-ZA
!    USASCII	0x2c09 # English (Trinidad and Tobago)		en-TT
!    USASCII	0x0809 # English (United Kingdom)		en-GB
!    USASCII	0x0409 # English (United States)		en-US
!    USASCII	0x3009 # English (Zimbabwe)			en-ZW

  {SOLARIS}
     ENGLISH	en
!    ENGLISH	en_AU
     ENGLISH	en_AU.ISO8859-1
     ENGLISH	en_CA.ISO8859-1
!    ENGLISH	en_CA
!    ENGLISH	en_UK
!    ENGLISH	en_US
     ENGLISH	en_US.ISO8859-1
     ENGLISH	en_US.ISO8859-15
     ENGLISH	en_US.ISO8859-15@euro
!    ENGLISH	en_IE
     ENGLISH	en_IE.ISO8859-1
     ENGLISH	en_IE.ISO8859-15
     ENGLISH	en_IE.ISO8859-15@euro
!    ENGLISH	en_NZ
     ENGLISH	en_NZ.ISO8859-1
!    ENGLISH	en_GB
     ENGLISH	en_GB.ISO8859-1
     ENGLISH	en_GB.ISO8859-15
     ENGLISH	en_GB.ISO8859-15@euro
!    JAPANESE	ja
     JAPANESE	ja_JP
     JAPANESE	ja_JP.eucJP
     JAPANESE	japan
     JAPANESE	japanese
!    FRENCH     fr
     FRENCH     fr_FR
     FRENCH     fr_FR.ISO8859-1
     FRENCH     fr_FR.ISO8859-15
     FRENCH     fr_FR.ISO8859-15@euro
!    FRENCH     fr_BE
     FRENCH     fr_BE.ISO8859-1
     FRENCH     fr_BE.ISO8859-15
     FRENCH     fr_BE.ISO8859-15@euro
!    FRENCH     fr_CA
     FRENCH     fr_CA.ISO8859-1
!    FRENCH     fr_CH
     FRENCH     fr_CH.ISO8859-1
!    GERMAN     de
     GERMAN     de_DE
     GERMAN     de_DE.ISO8859-15@euro
     GERMAN     de_AT
     GERMAN 	de_DE.ISO8859-1
     GERMAN 	de_DE.ISO8859-15
     GERMAN 	de_AT.ISO8859-15@euro
     GERMAN 	de_AT.ISO8859-15
     GERMAN 	de_AT.ISO8859-1
     GERMAN 	de_CH.ISO8859-1
!    GERMAN 	de_CH
!    ITALIAN	it
     ITALIAN	it_IT
     ITALIAN    it_IT.ISO8859-15
     ITALIAN    it_IT.ISO8859-15@euro
     ITALIAN	it_CH
!    SPANISH	es
!    SPANISH	es_AR
!    SPANISH	es_BO
!    SPANISH	es_CL
!    SPANISH	es_CO
!    SPANISH	es_CR
!    SPANISH	es_EC
!    SPANISH	es_GT
!    SPANISH	es_MX
!    SPANISH	es_NI
!    SPANISH	es_PA
!    SPANISH	es_PE
!    SPANISH	es_PY
!    SPANISH	es_SV
!    SPANISH	es_UY
!    SPANISH	es_VE
     SPANISH    es_ES.ISO8859-15
     SPANISH    es_ES.ISO8859-15@euro
!    KOREAN 	ko
!    KOREAN 	korean
     KOREAN 	ko_KR.EUC
     CHINESE_TW	zh_TW
     CHINESE_TW	tchinese
!    CHINESE_TW	zh_TW.BIG5	// Solaris 2.6
!    CHINESE_TW	zh_TW.big5	// Solaris 2.5.1
!    CHINESE_TW	big5		// Solaris 2.5.1
!    CHINESE_CN	zh
!    CHINESE_CN	zh_CN.EUC
!    CHINESE_CN	chinese
!    HEBREW 	he
     HEBREW 	he_IL
     HEBREW 	he_IL.ISO8859-8
!    GREEK      el
!    GREEK      el_GR
!    GREEK      el_GR.ISO8859-7
!    GREEK      el_GR.ISO8859-7@euro
!    TURKISH    tr
!    TURKISH    tr_TR
!    TURKISH    tr_TR.ISO8859-9
!    CZECH      cz
!    CZECH      cs_CZ
!    CZECH      cs_CZ.ISO8859-2
!    POLISH     pl
!    POLISH     pl_PL
!    POLISH     pl_PL.ISO8859-2
!    HUNGARIAN  hu
!    HUNGARIAN  hu_HU
!    HUNGARIAN  hu_HU.ISO8859-2
!    SLOVENIAN  sl
!    SLOVENIAN  sl_SI
!    SLOVENIAN  sl_SI.ISO8859-2
!    PORTUGUESE pt
!    PORTUGUESE pt.ISO8859-15
!    PORTUGUESE pt_PT
!    PORTUGUESE pt_PT.ISO8859-1
!    PORTUGUESE pt_PT.ISO8859-15
!    PORTUGUESE pt_PT.ISO8859-15@euro
!    SLOVAK     sk
!    SLOVAK     sk_SK
!    SLOVAK     sk_SK.ISO8859-2
!    RUSSIAN    ru.koi8-r
!    RUSSIAN    ru_RU
!    RUSSIAN    ru_RU.ANSI1251
!    RUSSIAN    ru_RU.ISO8859-5
!    RUSSIAN    ru_RU.KOI8-R


  {HP}
!    ENGLISH	C.iso88591
!    ENGLISH	english
!    ENGLISH	english.iso88591
!    ENGLISH	en_GB.iso88591
!    ENGLISH	en_US.iso88591
!    JAPANESE	japanese.euc
!    JAPANESE	ja_JP.eucJP
!    FRENCH 	c-french
!    FRENCH 	c-french.iso88591
!    FRENCH 	french
!    FRENCH 	french.iso88591
!    FRENCH 	fr_CA.iso88591
!    FRENCH 	fr_FR.iso88591
     FRENCH 	fr_FR.iso885915@euro
!    GERMAN     german
!    GERMAN 	german.iso88591
     GERMAN 	de_DE.iso885915@euro
!    GERMAN 	de_DE.iso88591
!    ITALIAN	italian
!    ITALIAN	italian.iso88591
!    ITALIAN	it_IT.iso88591
     ITALIAN	it_IT.iso885915@euro
!    SPANISH	spanish
!    SPANISH	spanish.iso88591
!    SPANISH	es_ES.iso88591
     SPANISH	es_ES.iso885915@euro
!    KOREAN 	ko_KR.eucKR
!    CHINESE_TW	zh_TW.big5
!    CHINESE_CN	zh_CN.hp15CN
!    HEBREW 	iw_IL.hebrew8
!    HEBREW 	iw_IL.iso88598
!    GREEK      el_GR.greek8
!    GREEK      el_GR.iso88597
!    TURKISH    tr_TR.turkish8
!    TURKISH    tr_TR.iso88599
!    CZECH      cs_CZ.iso88592
!    POLISH     pl_PL.iso88592
!    HUNGARIAN  hu_HU.iso88592
!    SLOVENIAN  sl_SI.iso88592
!    PORTUGUESE pt_PT.iso88591
!    PORTUGUESE pt_PT.iso885915@euro
!    SLOVAK     sk_SK.iso88592

  {SGI}
!    ENGLISH	en
     ENGLISH    en_US.ISO8859-15
     ENGLISH    en_GB.ISO8859-15
     ENGLISH    en_CA.ISO8859-15
     ENGLISH    en_AU.ISO8859-15
!    ENGLISH    en_AU
!    ENGLISH	en_CA
!    ENGLISH	en_US
!    JAPANESE	ja_JP.EUC
!    FRENCH 	fr
!    FRENCH 	fr_BE
     FRENCH     fr_BE.ISO8859-15
     FRENCH     fr_FR.ISO8859-15
!    FRENCH     fr_CA
     FRENCH     fr_CA.ISO8859-15
     FRENCH     fr_CH.ISO8859-15
!    FRENCH 	fr_CH
!    GERMAN 	de
!    GERMAN 	de_AT
!    GERMAN 	de_CH
     GERMAN     de_AT.ISO8859-15
     GERMAN     de_CH.ISO8859-15
     GERMAN     de_DE.ISO8859-15
!    ITALIAN    it
!    ITALIAN	it_CH
     ITALIAN    it_CH.ISO8859-15
     ITALIAN	it_IT.ISO8859-15
!    SPANISH	es
!    SPANISH	es_AR
     SPANISH    es_AR.ISO8859-15
     SPANISH    es_ES.ISO8859-15
!    SPANISH	es_MX
     SPANISH    es_MX.ISO8859-15
!    KOREAN 	ko_KR.euc
!    CHINESE_TW	zh_TW.big5
!    CHINESE_CN	zh_CN.gbk
!    CHINESE_CN	zh_CN.eucgbk
!    CHINESE_CN	zh_CN.ugb
!    HEBREW 	iw
!    GREEK      el
!    TURKISH    tr
!    CZECH      cz
!    POLISH     pl
!    HUNGARIAN  hu
!    SLOVENIAN  sl
!    PORTUGUESE pt
!    PORTUGUESE pt_PT.ISO8859-15
!    SLOVAK     sk

  {LINUX}
     ENGLISH    en_CA
     ENGLISH    en_GB
     ENGLISH    en_GB.iso885915
     ENGLISH    en_NZ
     ENGLISH    en_IE
     ENGLISH    en_IE@euro
     ENGLISH    en_US
     ENGLISH    en_US.iso885915
     SPANISH    es_ES@euro
     SPANISH    es_ES
     SPANISH    spanish
     SPANISH	es_AR
     SPANISH	es_BO
     SPANISH	es_CL
     SPANISH	es_CO
     SPANISH	es_CR
     SPANISH	es_DO
     SPANISH	es_HN
     SPANISH	es_EC
     SPANISH	es_GT
     SPANISH	es_MX
     SPANISH	es_NI
     SPANISH	es_PA
     SPANISH	es_PE
     SPANISH	es_PR
     SPANISH	es_PY
     SPANISH	es_SV
     SPANISH	es_US
     SPANISH	es_UY
     SPANISH	es_VE
     SPANISH    es_ES.ISO8859-15
     SPANISH    es_ES.ISO8859-15@euro
     FRENCH     fr_FR
     FRENCH     fr_FR@euro
     FRENCH     fr_BE
     FRENCH     fr_BE@euro
     FRENCH     fr_CA
     FRENCH     fr_CH
     FRENCH     fr_LU
     FRENCH     fr_LU@euro
     FRENCH     french
     GERMAN     german
     GERMAN     de_AT
     GERMAN     de_AT@euro
     GERMAN     de_DE
     GERMAN     de_DE@euro
     GERMAN     de_BE
     GERMAN     de_BE@euro
     GERMAN     de_CH
     GERMAN     de_LU
     GERMAN     de_LU@euro
     GERMAN     deutsch
     HEBREW     he_IL
     HEBREW     hebrew
     ITALIAN    it_IT
     ITALIAN    it_IT@euro
     ITALIAN    it_CH
     ITALIAN    italian
     JAPANESE   ja_JP
     JAPANESE   ja_JP.eucjp
     JAPANESE   japanese
     JAPANESE   japanese.euc
     KOREAN     ko_KR
     KOREAN     ko_KR.euckr
     KOREAN     korean
     KOREAN     korean.euc
     CHINESE_TW zh_TW
     CHINESE_TW zh_TW.euctw
     CHINESE_CN zh_CN
     CHINESE_CN zh_CN.gbk
!    GREEK      el_GR
!    GREEK      greek
!    TURKISH    tr_TR
!    TURKISH    turkish
!    CZECH      cs_CZ
!    CZECH      czech
!    POLISH     pl_PL
!    POLISH     polish
!    HUNGARIAN  hu_HU
!    HUNGARIAN  hungarian
!    SLOVENIAN  sl_SI
!    SLOVENIAN  slovenian
!    SLOVENIAN  slovene
!    PORTUGUESE pt_PT
!    PORTUGUESE pt_PT@euro
!    PORTUGUESE portuguese
!    SLOVAK     sk_SK
!    SLOVAK     slovak


