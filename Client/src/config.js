const DEBUG = false;
const CONFIG = Object.freeze(
{
    unknown: "Unknown",
    defaultLabel: 'STARRED',
    // Derived from gmailAPI.getLabels()
    labels: ["CHAT", "SENT", "INBOX", "IMPORTANT", "TRASH", "DRAFT", "SPAM", "CATEGORY_FORUMS", "CATEGORY_UPDATES", "CATEGORY_PERSONAL", "CATEGORY_PROMOTIONS", "CATEGORY_SOCIAL", "STARRED", "UNREAD"],
    displayBodiesEvent: "displayBodies",
    reloadInboxEvent: "reloadInbox",
    rowClassName: "Item",
    threadIdClassName: "threadId",   
})

export { CONFIG, DEBUG };
