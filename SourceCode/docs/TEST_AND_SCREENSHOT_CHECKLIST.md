# Evidence checklist - replace placeholders with actual captures

## Database
- [ ] Query counts: Item 20; Agent 20; Order 20; OrderDetail 28; Users 20 (on untouched seed DB)
- [ ] ERD showing PK and FK connections
- [ ] SELECT data for Item, Agent, Order, OrderDetail, Users
- [ ] Restrictions and CHECK constraints verified

## Admin Back-Office (WinForms)
- [ ] Admin signs in; Agent user is rejected
- [ ] Wrong password; lockout after 5 attempts (use a fresh disposable demo account)
- [ ] Add/Edit/Delete Item (avoid deleting referenced rows)
- [ ] Add/Edit/Delete Agent (avoid deleting referenced rows)
- [ ] Receive web order, change status
- [ ] Export actual Shipping Manifest PDF
- [ ] Dashboard (best seller, low stock, agent revenue)

## Agent Portal
- [ ] Agent logs in, Admin is rejected
- [ ] View catalog and stock amounts
- [ ] Place a multi-line order, view successful invoice
- [ ] View stock before vs after order
- [ ] Order exceeding stock rejected; transaction rollback verified in SQL
- [ ] My Orders shows only that Agent's data, no other Agent's order
- [ ] Desktop AND mobile-width views

## Tests and Config
- [ ] `dotnet test` terminal screenshot showing real totals (do not invent pass rate)
- [ ] App.config and appsettings.json, secrets hidden
- [ ] GitHub branches, pull requests, review comments, real commit graph
- [ ] Actual 3-5 minute video URL
- [ ] Names, IDs, % contribution totaling 100% match README and report
