# qrbillcodefunction
QR Bill Code Function wrappt das Paket Codecrete.SwissQRBill.Generator in eine Azure Function, welche eine Swiss QR Bill generiert. Dabei kann der Output ein svg/xml sein oder eine PNG Datei.

Der Aufruf auf den Endpoint sieht dabei wie folgt aus:
POST: https://#dein-azure-endpoint#.azurewebsites.net/api/GenerateQRBill?code=#DEIN_FUNCTION_CODE#==

Als Payload musst du folgendes übergeben:
```json
{
	 	"account": "CHXXXXXXXXXXXXXXXXX", //Gültige IBAN des Empfängers
		"creditor": {                     //Adresse zur IBAN
			"name":"Dein Debitor",
			"street":"Vogelsangstrasse",
			"houseNumber": "9",
			"postalCode":"8307",
			"town":"Effretikon",
			"countryCode":"CH"
		},
		"debitor": {                   //Adresse für den Sender
			"name":"Hans Muster",
			"street":"Geren 1234",
			"postalCode":"8317",
			"town":"Tagelswangen",
			"countryCode":"CH"
		},
    "currency": "CHF",
    "amount": 120.45,                  //Der Betrag ist optional
    "referenceNumber": "Eine gültige ESR Referenznummer", //Die Referenznummer ist Optional
    "infoText": "Hello World"
}
```
Achtung QR Rechnungen mit Refernznummern brauchen ein Konto, welches für ESR Referenznummern aktiviert ist.

## Credits:
Die Hauptarbeit wurde durch https://github.com/manuelbl/SwissQRBill.NET erledigt.
