import { CONFIG } from './config.js'

window.onload = () =>
{
    
    for(let i = 1; i<200; i++)
    {
        let row = document.createElement("tr");
        row.setAttribute("tabIndex", i);
        row.className = "Item";
        
        // Create a 3-item array of <td> elements
        let columns = [...Array(CONFIG.tableColumns).keys()].map( () => document.createElement("td") );  
        
        columns[0].innerText = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        columns[1].innerText = "WOOOW";
        columns[2].innerText = "WOOOW";

        for(let c of columns) { row.appendChild(c); }
        document.querySelector("#mailList > tbody").appendChild(row);
            
    }

}
