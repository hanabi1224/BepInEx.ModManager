<template>
  <div>
    <a-avatar
      v-if="game.getIcon()"
      :src="game.getIcon()"
    ></a-avatar>
    {{ game.getName() }} ({{ game.getIs64bit() ? '64bit' : '32bit' }}) - BIE
    <a-icon
      :type="bieStatusIcon"
      :style="bieStatusIconStyle"
    ></a-icon>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { grpcClient } from './../utils';
import { GameInfo, InstallBIERequest } from './../generated/Game_pb';
import { shell } from 'electron';

@Component({
    components: {},
})
export default class GameTitle extends Vue {
    @Prop()
    game!: GameInfo;

    openPath() {
        return shell.openPath(this.game.getPath());
    }

    get bieStatusIcon(){
      return this.game.getIsbieinstalled() ? 'check' : 'close';
    }

    get bieStatusIconStyle(){
      if(!this.game.getIsbieinstalled()){
        return 'color:red';
      } 
      if (this.game.getIsbieinitialized()){
        return 'color:green';
      }
      return 'color:orange';
    }
}
</script>

<style>
</style>
