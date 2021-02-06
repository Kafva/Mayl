<template>
    <table>
        <thead>
            <th class="nf nf-mdi-mail_ru"></th>
            <th class="nf nf-mdi-view_headline"></th>
            <th class="nf nf-fa-calendar"></th>
        </thead>
        <tbody>
            <!-- NOTE that thread.id is not a manually set attribute but an attribute
            generated for the purpose of the loop so that Vue can keep track of the elements -->
            <email-row  v-for="thread in threads"
                v-bind:key="thread.id"
                v-bind:thread="thread"
            />
        </tbody>
    </table>
</template>

<script>

import {DEBUG}  from '../src/config.js';
import EmailRow from '../components/EmailRow.vue';

export default {
    // Component definitions are similar to root Vue() element definitions but cannot include an 'el:' selector
    // Since their implicit root is the <template> 
    name: 'email-table',
    
    components: 
    // Add the EmailRow component as a child
    {
        emailRow: EmailRow
    },
    
    data: function() {
        return {
            label: "STARRED",
            threads: null, 
        }
    },
    
    // Runs when a new component instance is created
    created() { this.fetchThreads(); },

    methods:
    {
        fetchThreads: async function()
        {
            let res = await fetch(`/me/mail?label=${this.label}`, {
                method: "GET",
                headers: {"Content-Type": "application/json"},
            });
            
            let body = null;
            try 
            { 
                body = await res.text();
                
                // Decode from base64 and then translate \u sequences into actual
                // glyphs with JSON.parse()
                this.threads = JSON.parse( atob(body)  );
                if(DEBUG) console.log(this.threads);
            }
            catch (e) { console.error(e); }
        },
    }
}
</script>

<style>

@keyframes fadeIn100 {
    from { opacity: 0; }
    to   { opacity: 1; }
}

table
{
    position: absolute;
    left: 5%;
    top: 6%;
    
    border-collapse: collapse;
    text-align: right;

    margin-left: 15px;
    margin-right: 15px;
    padding: 15px;
    
    /* Fade out 
    animation: fadeIn100 0.5s;

    visibility: visible;
    opacity: 1;
    transition: opacity 0.5s linear;
    */
}

thead
{
    margin-bottom: 5px;
    border-bottom: 1px solid rgba(65, 64, 64, 0.5)
}

td,th { padding-right: 14px;  }

</style>
