<template>
    <select id="accountSelect" v-model="selected"  @change="emitEmailTableEvent">
      <option v-for="account in accounts" :key="account.id">
        {{ account }}
      </option>
    </select>
</template>

<script>
import {CONFIG} from '../src/config.js'

export default {
    name: 'account-select',
    props: {},
    data: function() {
        return {
          selected: null,
          accounts: [],
        };
    },
    
    created() { this.setAccounts(); },

    methods:
    {
      setAccounts: async function()
      {
        let res = await fetch(`/accounts`, { method: "GET"});
        try 
        { 
          this.accounts = (await res.text()).split(",");
          this.selected = this.accounts[0];
        }
        catch (e) { console.error(e); }
      },

      emitEmailTableEvent()
      // Emit a signal to the EmailTable, notifying it that  
      // the account has changed and that it should reload 
      {
        this.$root.$emit(CONFIG.reloadInboxEvent, this.account, "");
      }
    }
}
</script>

<style scoped>

select
{
  max-width: 200px;
}

</style>

