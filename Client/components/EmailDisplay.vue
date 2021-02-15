<template>
<div id="emailDisplay" :style="collapseCSS">
    <i class="btn nf nf-mdi-arrow_collapse_left" @click="hide"></i> 
    <b>{{ subject }}</b>
    <div v-for="message in messages"
        v-bind:key="message.id">
        <div class="emailBody">
            <p>{{ message.date }}</p>
            <!-- <div v-html="sanitiseBody(message.body)"></div> -->
            <iframe 
                id="emailFrame"
                height="600" width="700"
                frameborder="0" 
                allowtransparency="true"
                scrolling="yes" 
                style="background-color: white !important"
                :src="getURIFromText(message.body)">
            </iframe> 
        </div>
    </div>
</div>
</template>

<script>

import {CONFIG}  from '../src/config.js';
import * as Functions  from '../src/functions.js';


export default {
    name: 'email-display',
    
    data: function() {
        return {
            threadId: null,
            messages: [],
            minify: true, 
        }
    },
    
    computed: 
    // Hide/Unhide the display when 'minify' changes
    {
        collapseCSS(){ return Functions.collapseCSS(this.minify); },
        subject()
        {
            return this.messages.length > 0 ? this.messages[0].subject :
                CONFIG.unknown;
        }
    },
   
    mounted()
    { 
        this.$root.$on(CONFIG.displayBodiesEvent, (threadId) =>
        // Event listener for clicks in the EmailTable
        {
            // Update the minify property to display the body
            this.minify = false;

            this.threadId = threadId;
            this.fetchThreadMessages();
        });
    },

    methods:
    {
        fetchThreadMessages: async function()
        {
            let res = await fetch(`/${Functions.getSelected(CONFIG.accountSelector)}/thread?id=${this.threadId}`, {
                method: "GET",
            });
            
            let body = null;
            try 
            { 
              body = await res.text();
              
              // Decode from base64 and then translate \u sequences into actual
              // glyphs with JSON.parse()
              this.messages = JSON.parse(atob(body));
              
              // Sanitise the date for each message
              this.messages.forEach( (t,i, messages ) => {
                  messages[i].date = Functions.getDate( messages[i].date );
              });
            }
            catch (e) { console.error(e); }
        },
        
        sanitiseBody(body)
        {
            // Note the use of /g for replace all
            return body.replaceAll(/<style/gi, "<style scoped")
                .replaceAll(/<script.*<\/script>/gi, "")
                .replaceAll(/onerror=".*"/gi, "")
                .replaceAll(/!important/gi, "");
        },

        getURIFromText(body)
        {
            return `data:text/html;charset=utf-8,${encodeURIComponent( this.sanitiseBody(body)  )}`;
        },

        hide()
        {
            this.minify = true;
            this.$root.$emit(CONFIG.hideBodiesEvent);
        },
    }
}
</script>

<style>
#emailDisplay
{
    width: fit-content;
    height: fit-content;
    /* Display none by default and change to inline-block when an event is caught */ 
    display: none;
    
    animation: fadeIn100 0.5s;

    visibility: visible;
    opacity: 1;
    transition: opacity 0.5s linear;
}

#emailDisplay > i { display: inline-block; font-size: 30px; padding-right: 20px; }
#emailDisplay > b { display: inline-block; font-size: 30px; max-width: 500px; }

.emailBody
{
    display: inline-block;
    
    /* Important to avoid all text from appearing on a single line */
    white-space: wrap;
    overflow-wrap: break-word;
    width: 100%;
    height: 100%;
}


</style>
