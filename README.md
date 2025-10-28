# BS
````````

This is the code block that represents the suggested code change:

````````markdown
- `orderItems` stores serialized items in this format:
  `ID,Name,Rate,CQFT,GST,Amount||ID,Name,Rate,CQFT,GST,Amount`

Example value:
`1,Widget A,100.00,2,18,236.00||2,Widget B,50.00,1,18,59.00`

## Printing
Printing uses `BillRepository.CreateInvoiceFlowDocument` and `PrintDialog`. Ensure the app has UI thread access when showing `PrintDialog`. Images are loaded from the working directory (example: `213.png`); adjust the path if needed.

## Troubleshooting

- Duplicate type / CS0101: `The namespace 'BS' already contains a definition for 'BillItem'`
  - You may have `BillItem` defined more than once (for example `BillItem.cs` and another file or auto-generated signature). Solutions:
    1. Remove or rename the duplicate class file (keep one `BillItem`).
    2. Move one of the classes into a different namespace (e.g., `BS.Models`) and update references.
    3. If both are intended to be parts of the same logical type, consider `partial class BillItem` across files (both files must declare `partial` and same namespace).
  - Common places to check: `BillItem.cs`, `BillModel.cs`, any designer/auto-generated files or duplicate copies in the project.
  - In Visual Studio, use __Edit > Find and Replace > Find in Files__ or search the solution for `class BillItem`.

- Nulls or parsing issues when loading `orderItems`
  - Validate `orderItems` content; `ParseOrderItems` expects CSV-like fields and `||` separators.
  - Use defensive parsing and logging if any rows fail to parse.

- Printing exceptions
  - Ensure `CreateInvoiceFlowDocument` runs on UI thread and images exist at expected paths.

## Contributing
- Fork, branch, add tests, open a PR.
- Keep WPF UI changes separate from repository/data access concerns.

## Notes
- Database path default: `Data\store.db` (relative to app working directory). You can provide an alternate path via the `BillRepository(string dbPath)` constructor.
- Project targets WPF and .NET 8.0 — update project TFM if needed.

## License
Add your preferred license file (`LICENSE`) to the repo.

````````

# Response
````````markdown
# BS
````````

This is the code block that represents the suggested code change:

````````markdown
- `orderItems` stores serialized items in this format:
  `ID,Name,Rate,CQFT,GST,Amount||ID,Name,Rate,CQFT,GST,Amount`

Example value:
`1,Widget A,100.00,2,18,236.00||2,Widget B,50.00,1,18,59.00`

## Printing
Printing uses `BillRepository.CreateInvoiceFlowDocument` and `PrintDialog`. Ensure the app has UI thread access when showing `PrintDialog`. Images are loaded from the working directory (example: `213.png`); adjust the path if needed.

## Troubleshooting

- Duplicate type / CS0101: `The namespace 'BS' already contains a definition for 'BillItem'`
  - You may have `BillItem` defined more than once (for example `BillItem.cs` and another file or auto-generated signature). Solutions:
    1. Remove or rename the duplicate class file (keep one `BillItem`).
    2. Move one of the classes into a different namespace (e.g., `BS.Models`) and update references.
    3. If both are intended to be parts of the same logical type, consider `partial class BillItem` across files (both files must declare `partial` and same namespace).
  - Common places to check: `BillItem.cs`, `BillModel.cs`, any designer/auto-generated files or duplicate copies in the project.
  - In Visual Studio, use __Edit > Find and Replace > Find in Files__ or search the solution for `class BillItem`.

- Nulls or parsing issues when loading `orderItems`
  - Validate `orderItems` content; `ParseOrderItems` expects CSV-like fields and `||` separators.
  - Use defensive parsing and logging if any rows fail to parse.

- Printing exceptions
  - Ensure `CreateInvoiceFlowDocument` runs on UI thread and images exist at expected paths.

## Contributing
- Fork, branch, add tests, open a PR.
- Keep WPF UI changes separate from repository/data access concerns.

## Notes
- Database path default: `Data\store.db` (relative to app working directory). You can provide an alternate path via the `BillRepository(string dbPath)` constructor.
- Project targets WPF and .NET 8.0 — update project TFM if needed.

## License
Add your preferred license file (`LICENSE`) to the repo.