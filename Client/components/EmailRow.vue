<template>
    <tr class="Item">
        <td>       {{ sender }}         </td>
        <td>       {{ thread.snippet }} </td>
        <td>       {{ date }}           </td>
        <td hidden>{{ thread.threadId }}</td>
        <td @click="archiveMessage" class="btn nf nf-mdi-archive">  </td>
        <td @click="deleteMessage"  class="btn nf nf-fa-trash">      </td>
    </tr>
</template>

<script>
import { CONFIG } from '../src/config.js';

export default {
    // Component definitions are similar to root Vue() element definitions but cannot include an 'el:' selector
    // Since their implicit root is the <template> 
    name: 'email-row',
    
    // Props defines the parameters which we can pass to a component when
    // creating a new instance, e.g. <hello title="xD"/>
    /* NOTE that HTML attributes are case-insensitve and
    camelCase props should be referenced with their kebab-case equvivalent 
    threadId ==> thread-id */
    props: { thread: Object },
    
    computed:
    {
        sender: function()
        { 
            
            return this.thread.emails.length > 0 ? 
                this.thread.emails[0].sender : CONFIG.unknown; 
        },
        date: function()
        {
            return this.thread.emails.length > 0 ? 
                this.thread.emails[0].date : CONFIG.unknown; 
        }
    },
    methods:
    {
        archiveMessage: function(event)
        {
            console.log("TODO");
        },
        deleteMessage: function(event)
        {
            console.log("TODO");
        },
    }
}
</script>

<style>
.Item
{
    opacity: 1.0;
    max-height: 18px;
    margin-bottom: 5px;
    
    border-bottom: 1px solid rgba(203, 198, 198, 0.5)
}

.Item:hover
{
    background-color: rgba(69, 69, 75, 0.5);
    opacity: 0.5;
    cursor: pointer;
}


.Item > td
{
    /* Set an upper limit for the width of column text in the playlist */
    text-overflow: ellipsis;
    overflow: hidden;
    max-width: 250px;
}


</style>
