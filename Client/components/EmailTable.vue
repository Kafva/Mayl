<template>
    <table>
        <thead>
            <th class="nf nf-mdi-mail_ru"       :style="collapseCSS"></th>
            <th class="nf nf-mdi-view_headline"></th>
            <th class="nf nf-fa-calendar"       :style="collapseCSS"></th>
            <th></th>
            <th></th>
        </thead>
        <tbody>
            <!-- NOTE that thread.id is not a manually set attribute but an attribute
            generated for the purpose of the loop so that Vue can keep track of the elements -->
            <email-row :minify="minify" v-for="thread in threads" 
                v-bind:key="thread.id"
                v-bind:thread="thread">
            </email-row>
        </tbody>
    </table>
</template>

<script>

import {CONFIG, DEBUG}  from '../src/config.js';
import * as Functions  from '../src/functions.js';
import EmailRow from '../components/EmailRow.vue';

export default {
    // Component definitions are similar to root Vue() element definitions but cannot include an 'el:' selector
    // Since their implicit root is the <template> 
    name: 'email-table',
    
    props: {},

    components: 
    // Add the EmailRow component as a child
    {
        emailRow: EmailRow
    },
    
    data: function() {
        return {
            label: CONFIG.defaultLabel,
            minify: false,
            threads: null, 
        }
    },
   
    computed:
    {
        collapseCSS: function(){ return Functions.collapseCSS(this.minify); }
    },

    mounted()
    { 
        // Register a listener for the reloadInbox event sent by the <label-select> component
        this.$root.$on(CONFIG.reloadInboxEvent, (newLabel) =>
        {
            console.log("Got event!!!");
                        
            if(newLabel) this.label = newLabel;
            this.fetchThreads(); 
        });
        
        this.$root.$on(CONFIG.displayBodiesEvent, () =>
        // Event listener to minify email entries when an entry is clicked
        // the change will propogate down to the EmailRow since we pass minify as a prop
        {
            this.minify = true;
        });

        this.fetchThreads(); 
    },

    methods:
    {
        fetchThreads: async function()
        {
            let res = await fetch(`/me/mail?label=${this.label}`, {
                method: "GET",
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


table
{
    display: inline-block;
    width: fit-content;
    height: fit-content;
    min-height: fit-content;
    border-collapse: collapse;
    text-align: right;

    margin-left: 15px;
    margin-right: 15px;
    padding: 15px;
    
    animation: fadeIn100 0.5s;

    visibility: visible;
    opacity: 1;
    transition: opacity 0.5s linear;
}

thead
{
    margin-bottom: 5px;
    border-bottom: 1px solid rgba(203, 198, 198, 0.5)
}

td,th { padding-right: 14px;  }

</style>
