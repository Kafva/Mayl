<template>
    <tr :class="rowClassName" @click="emitEmailDisplayEvent">
        <td :style="collapseCSS">       {{ sender }}         </td>
        <td>                            {{ thread.snippet }} </td>
        <td :style="collapseCSS">       {{ date }}           </td>
        <td :class="threadIdClassName" hidden>{{ thread.threadId }}            </td>
        <td @click="archiveMessage" class="btn nf nf-mdi-archive">   </td>
        <td @click="deleteMessage"  class="btn nf nf-fa-trash">      </td>
    </tr>
</template>

<script>
import { CONFIG } from '../src/config.js';
import * as Functions  from '../src/functions.js';

export default {
    // Component definitions are similar to root Vue() element definitions but cannot include an 'el:' selector
    // Since their implicit root is the <template> 
    name: 'email-row',
    
    // Props defines the parameters which we can pass to a component when
    // creating a new instance, e.g. <hello title="xD"/>
    /* NOTE that HTML attributes are case-insensitve and
    camelCase props should be referenced with their kebab-case equvivalent 
    threadId ==> thread-id */
    props: { thread: Object, minify: Boolean },
    data: function (){
        return {
            threadIdClassName: CONFIG.threadIdClassName,
            rowClassName: CONFIG.rowClassName,
        }
    },
   
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
        },
    
        collapseCSS: function(){ return Functions.collapseCSS(this.minify); }

    },
    methods:
    {
        archiveMessage: function(event)
        {
            console.log("Archive!");
            event.cancelBubble = true;
            console.log(event);
        },
        deleteMessage: function(event)
        {
            console.log("Delete!");
            event.cancelBubble = true;
            console.log(event);
        },
        emitEmailDisplayEvent: function(event)
        {
            // Extract the tr.Item element from the click-event
            let index = event.path.findIndex( (p) => p.className == CONFIG.rowClassName );
            if (index != -1)
            {                
                // Extract the value of the hidden threadId field and send it with the signal
                this.$root.$emit(CONFIG.displayBodiesEvent, 
                    event.path[index].querySelector(`.${CONFIG.threadIdClassName}`).innerText  );
            }
            else{ console.error(`Could not find ${CONFIG.rowClassName} in ${event.path}`); }
        } 
    }
}
</script>

<style>
.Item
{
    /* Prevent column text from wrapping around */
    white-space: nowrap;
    opacity: 1.0;
    max-height: 18px;
    width: fit-content;
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
    max-width: 200px;
}

td.btn { z-index: 900; }

</style>
