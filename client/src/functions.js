import { CONFIG } from './config.js'

const fetchThreads = async () =>
{
    // https://vuejs.org/v2/cookbook/using-axios-to-consume-apis.html
    let res = await fetch("/me/mail?label=STARRED", {
        method: "GET",
        headers: {"Content-Type": "application/json"},
    });
    
    let body = null;
    try 
    { 
        body = await res.text();
        console.log(JSON.parse(body));
        return JSON.parse(body);
    }
    catch (e) { console.error(e,body); }
}

const getBodyOfMessage = (threadId) =>
{
    return "wow";
}


export { fetchThreads, getBodyOfMessage };
