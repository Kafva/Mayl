<template>
<div id="emailDisplay" :style="collapseCSS">
    <div  v-for="message in messages"
        v-bind:key="message.id">
        <h1>{{ message.subject }}</h1>
        <i>{{ message.date }}</i>
        <p class="htmlBody" v-html="message.body"></p>
    </div>
</div>
</template>

<script>

import {CONFIG}  from '../src/config.js';
import * as Functions  from '../src/functions.js';

export default {
    name: 'email-display',
    
    props: { minify: Boolean },

    data: function() {
        return {
            threadId: null,
            messages: [], 
        }
    },
   
    mounted()
    { 
        this.$root.$on(CONFIG.displayBodiesEvent, (threadId) =>
        // TODO let the EmailRow signal root instead and let root mount/remount
        // the EmailDisplay to align the flexbox
        // Event listener for clicks in the EmailTable
        {
            // Update the minify property
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
        
        collapseCSS: function(){ return Functions.collapseCSS(this.minify); }
    }
}
</script>

<style>
#emailDisplay
{
    width: 80%;
    height: fit-content;
    display: inline-block;
    
    animation: fadeIn100 0.5s;

    visibility: visible;
    opacity: 1;
    transition: opacity 0.5s linear;
}

.htmlBody
{
    width: fit-content;
    height: 30%;
}


</style>
