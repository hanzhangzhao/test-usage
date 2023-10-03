module CertificateUsage.Tests.Stubs.CarrierRateModifiedEvent

let event =
    """
        {
           "carrier_rate": "1.23",
           "effective_date": "2023-07-20",
           "option": "Option",
           "coverage": "Coverage",
           "product_line": "ProductLine",
           "dc_option": null,
           "price_per": 1,
           "policy_number": "PolicyNumber",
           "carrier": "Carrier",
           "changed_by": {
             "id": 2,
             "name": "ChangedBy.Name"
           }
        }
        """

let metadata =
    """
    {
        "version": "Version",
        "create_date": 1646006400
    }
    """
