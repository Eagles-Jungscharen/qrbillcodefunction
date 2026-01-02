namespace EaglesJungscharen.Azure.Model {
    public class InputBill {
        public required string Account {set;get;}
        public required InputAddress Creditor {set;get;}
        public required InputAddress Debitor {set;get;}
        public required string Currency {set;get;}
        public decimal ?Amount {set;get;}
        public string? ReferenceNumber {set;get;}
        public string? InfoText {set;get;}
    }
}