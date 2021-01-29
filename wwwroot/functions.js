import { CONFIG } from './config.js'

const fetchThreads = async () =>
{
    // https://vuejs.org/v2/cookbook/using-axios-to-consume-apis.html
    let res = await fetch("/me/mail?label=STARRED", {
        method: "GET",
        //headers: {"Content-Type": "application/json"},
    });
    
    let body = null;
    try 
    { 
        body = await res.text();
        body = body.replace(/\\\\u003C/, "");
        body = body.replace(/\\\\u003E/, "");
        
        // Something is fcked with the output from the server
        // double parse...
        let _json = JSON.parse(body) 
        _json = JSON.parse(_json) 
        console.log(_json, _json.threadId);
        return _json;
    }
    catch (e) { console.error(e,body); }
    
    //let el = {
    //    subject: "subject",
    //    sender: "sender",
    //    date: "2020" 
    //};
    //return [...Array(20).keys()].map( () => el ); 
}


export { fetchThreads };
