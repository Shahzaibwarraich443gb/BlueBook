import { AccountingBlueBookTemplatePage } from './app.po';

describe('AccountingBlueBook App', function() {
  let page: AccountingBlueBookTemplatePage;

  beforeEach(() => {
    page = new AccountingBlueBookTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
