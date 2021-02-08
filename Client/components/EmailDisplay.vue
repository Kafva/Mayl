<template>
<div id="emailDisplay" :style="collapseCSS">
    <i class="btn nf nf-mdi-arrow_collapse_left" @click="hide"></i>
    <div v-for="message in messages"
        v-bind:key="message.id">
        <div class="emailBody">
            <b>{{ message.subject }}</b>
            <p>{{ message.date }}</p>
            <div v-html="sanitiseBody(message.body)"></div>
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
        collapseCSS: function(){ return Functions.collapseCSS(this.minify); }
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
            let res = await fetch(`/me/thread?id=${this.threadId}`, {
                method: "GET",
            });
            
            let body = null;
            try 
            { 
              body = await res.text();
              
              // Decode from base64 and then translate \u sequences into actual
              // glyphs with JSON.parse()
              this.messages = JSON.parse(atob(body));
              console.log(this.messages);
            }
            catch (e) { console.error(e); }
        },
        
        sanitiseBody(body)
        // Scope any style tags in the HTML of an email
        {
            return body.replaceAll("<style", "<style scoped");
        },

        hide()
        {
            this.minify = true;
            this.$root.$emit(CONFIG.hideBodiesEvent);
        }
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

.emailDisplay > i { font-size: 30px; float: right; }
.emailBody    > b { font-size: 30px; }

.emailBody
{
    display: inline-block;
    
    /* Important to avoid all text from appearing on a single line */
    white-space: wrap;
    overflow-wrap: break-word;
    width: 70%;
    height: 20%;
}


</style>
