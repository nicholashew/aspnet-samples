# SftpDataExport

This is a console application to demonstrate import csv and export manipulated csv to SFTP Remote Server with [SSH.NET library](https://github.com/sshnet/SSH.NET). 

For the sake of simplicity, we only use `PasswordAuthenticationMethod` to connect with SFTP in this example, you may consider including `PrivateKeyAuthenticationMethod` for production  .

## Private AppSettings Configuration

To prevent private settings being committed By Git, we decided to use the **"file"** Attribute to extending **<appSettings>** with an external file.

- Add the `PrivateAppSettings.config` file to the solution.

- Add the sensitive settings into `PrivateAppSettings.config` file, example:

```
<appSettings>
  <add key="SftpHost" value="127.0.0.1" />
  <add key="SftpPort" value="22" />
  <add key="SftpUsername" value="foo" />
  <add key="SftpPassword" value="bar" />
  <add key="SmtpServer" value="127.0.0.1" />
  <add key="SmtpPort" value="25" />
  <add key="SmtpAccount" value="" />
  <add key="SmtpPassword" value="" />
  <add key="SmtpEnableSsl" value="false"/>
</appSettings>
```

- Ensure the file is always copied by setting the copy option (properties of the file) with **Copy Always**.

- At this point you should have the config file deployed into your project directory everytime you compile the solution.

## Parser Settings

By adding config in `parser_settings.json`, we can configure the parser mapping to be export from the xml.

 - **name** (*string*) 
   The unique map name, also use as export filename.
   
 - **isLineItems** (*bool, optional*)
   Set this to `true` to parse to a listing result. Default: (`false`)
   
 - **lineItemPath** (*string, optional*)
   Set this to select line items (list of nodes) matching the XPath expression, this is mandatory if `isLineItems` equal to `true`.
   
 - **map** (*array*)
   The columns mapping settings
   
   - **name** (*string*) - The header column name for export
   
   - **xPath** (*string*) - The XPath expression to match the first XmlNode from XML Root
   
   - **attributeName** (*string, optional*) -  To define the node attribute for value retrieving, this is mandatory if `output` set as `Attribute`.
   
   - **output** (*string, optional*) - To define output value from the xml node. Default: (`InnerText`)
     - `InnerText` : Get the concatenated values of the node and all its child nodes
	 - `InnerXML` :  Get the markup representing only the child nodes of this node
     - `Value` : Get the value of the node.
     - `Attribute` : Get the attribute value of the node, required `attributeName`.
 
   - **isMandatory** (*bool*) - To set whether the value must be supplied for this field. Default: (`false`)

### Example

```json
{
  "Mapping": [
    {
      "name": "Order",
      "map": [
        {
          "name": "OrderNumber",
          "xPath": "order[@order-no]", //the XML Path expression
          "AttributeName": "order-no", //to define the attribute name for output
          "Output": "Attribute", //to define where the output value from
          "isMandatory": true
        },
        {
          "name": "OrderTotal",
          "xPath": "totals/gross-price",
          "isMandatory": false
        }
      ]
    },
    {
      "name": "Products",
      "isLineItems": true, //set true to get listing result
      "lineItemPath": "products/product", //set the line items XPATH
      "map": [
        {
          //set true so that we can select node from root instead of line items
          "isRootXPath": true, 
          "name": "OrderNumber",
          "xPath": "order[@order-no]",
          "AttributeName": "order-no",
          "Output": "Attribute",
          "isMandatory": true
        },
        {
          "isRootXPath": false,
          "name": "ProductID",
          "xPath": "product-id",
          "isMandatory": true
        },
        {
          "isRootXPath": false,
          "name": "ProductName",
          "xPath": "product-name",
          "isMandatory": true
        }
      ]
    }
  ]
}
```

## Contributing

Contributions are always welcome! If you'd like to contribute, please fork the repository and make changes as you'd like.