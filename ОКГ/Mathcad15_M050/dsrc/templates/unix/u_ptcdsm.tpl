//
// File: u_ptcdsm.tpl
//
// 05-Mar-02 $$1  ALG  Created
// 28-Mar-02 $$2  ALG  Rename file: "u_ptcdsqm.tpl" --> "u_ptcdsm.tpl"
// 15-Apr-02 $$3  ALG  Remove *_FEATURE_NAME lines
// 11-Jun-02 $$4  ALG  Add __AE_FLAG__ (Will be either " -ae" or "")
// 22-Nov-02 J-03-38  ALG  $$5  SPR 971401: Move most of contents to new
//                                 "common" file u_ptcdsm.inc
// 10-Aug-10 L-03-40  TWH  $$6  Add shlib.inc
//
#include:ptcdsm.inc
#include:shlib.inc

$prodir/$mc/obj/dsq $*
exit $status
