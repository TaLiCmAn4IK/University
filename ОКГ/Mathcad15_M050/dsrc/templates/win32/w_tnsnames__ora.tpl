//  4-FEB-97    H-01-22    IVK    $$1      Initial submission to PDM40.
// 29-SEP-99    I-02-09    APB    $$2      SDU & TDU added
// 19-Apr-00    I-03-28    TWH    $$3      Chg port to 1525 per sergey n.
// 12-Sep-02    J-01-50    TWH    $$4      Chg SDU/TDU per vlad
// 24-May-04    K-03-02    TWH    $$5      Chg port to 1521 per sergey n.
#######################
# Filename: tnsnames.ora 
#######################
INTRALINK =
  (DESCRIPTION =
    (SDU=32767)
    (TDU=32767)
    (ADDRESS_LIST =
       (ADDRESS =
          (PROTOCOL=TCP)
          (Host= __hostname__) #Hostname
          (Port= 1521)
       )
    )
    (CONNECT_DATA =
       (SID = ilnk)
    )
  )
