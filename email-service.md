### data format for email service: 

```json
{
  "eventType": "COURSE_PURCHASED",
  "to": ["user@email.com"],
  "subject": "...",
  "data": {"key":  "value"}
}
```

example:

```json
{
  "eventType": "COURSE_PURCHASED",
  "to": ["student@email.com"],
  "subject": "Course Purchased Successfully",
  "data": {
    "userName": "Omkar",
    "courseTitle": "Spring Boot Mastery",
    "amount": 499,
    "purchaseDate": "2026-01-28"
  },
  "attachments": [
    {
      "fileName": "receipt.pdf",
      "url": "https://storage/receipts/receipt-123.pdf"
    }
  ],
  "meta": {
    "priority": "HIGH",
    "channel": "EMAIL"
  }
}

```

sending email using email client
```java
emailClient.sendEmail(
        EmailEventRequest.builder()
        .eventType(NotificationType.COURSE_PURCHASED)
        .to(List.of(student.getEmail()))
        .subject("Course Purchased Successfully")
        .data(Map.of(
        "studentName", student.getFirstName(),
            "courseTitle", course.getTitle(),
            "pricePaid", booking.getPricePaid(),
            "currency", booking.getCurrency(),
            "purchaseTime", booking.getPurchaseTime(),
            "bookingId", booking.getId()
        ))
        .attachemnts(Map.of(
          "booking_invoice.pdf", booking.getInvoiceUrl()
        ))
                .build()
);

```
#### Event Types
for data field for different events\
create java map `Map<String, Object> ` with following fields

- INSTRUCTOR_APPROVAL_PENDING:
```json
{
  "firstName": "",
  "lastName": "",
  "email": "",
  "phoneNo": "",
  "experience": ""
}

```
- INSTRUCTOR_APPROVED
```json
{
  "firstName": ""
}
```
- INSTRUCTOR_PAYOUT
```json
{
  "firstName": "",
  "amount": "",
  "currency": "",
  "payoutDate": ""
}

```

- COURSE_PURCHASED
```json
{
  "studentName": "",
  "courseTitle": "",
  "pricePaid": "",
  "currency": "",
  "purchaseTime": "",
  "bookingId": ""
}

```

- PAYMENT_SUCCESS
```json
{
  "courseTitle": "",
  "amount": "",
  "currency": "",
  "paymentMethod": ""
}

```

- PAYMENT_FAILED
```json
{
  "courseTitle": "",
  "amount": "",
  "currency": ""
}

```

- PASSWORD_CHANGED
```json
{
  "userName": "",
  "changedAt": ""
}

```

- SIGN_IN_OTP
```json
{
  "otp": "",
  "validForMinutes": ""
}
```