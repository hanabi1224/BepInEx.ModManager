<template>
  <div>
    <a-modal
      :title="i18n('Config Editor')"
      :visible="visible"
      :cancel-text="i18n('Close')"
      :ok-text="i18n('Save')"
      @ok="save"
      @cancel="close"
    >
      <a-spin
        :spinning="saving"
        size="large"
      >
        <div class="container">
          <div ref="editor"></div>
        </div>
      </a-spin>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { ReadConfigRequest, WriteConfigRequest } from '../generated/Repo_pb';

import CodeMirror from 'codemirror';
import 'codemirror/mode/yaml/yaml.js';
import { grpcClient } from '../utils';

const theme = 'dracula';

@Component({
    components: {},
})
export default class ConfigEditorModal extends Vue {
    @Prop()
    show = false;

    saving = false;
    editor!: CodeMirror.Editor;

    get visible() {
        return !!this.show;
    }

    close() {
        this.editor.setValue('');
        this.$emit('close');
    }

    save() {
        this.saving = true;
        this.$message.info('Saving');
        const content = this.editor.getValue();
        const request = new WriteConfigRequest().setContent(content);
        grpcClient.writeConfig(request, {}, (err, response) => {
            try {
                if (err) {
                    this.$message.error(err.message);
                } else if (response.getSuccess()) {
                    this.$message.success('Done');
                } else {
                    this.$message.warn(response.getError());
                }
            } finally {
                this.saving = false;
                this.$emit('refreshGames');
                this.close();
            }
        });
    }

    i18n(text) {
        return i18next.t(text);
    }

    todo() {
        this.$message.warning(`Not implemented yet!`);
    }

    updated() {
        if (!this.editor) {
            console.log(this.$refs);
            const editorDiv = this.$refs['editor'] as HTMLDivElement;
            if (editorDiv) {
                this.editor = CodeMirror(editorDiv, {
                    theme: theme,
                    mode: 'yaml',
                });
            }
        }
        if (this.editor) {
            const request = new ReadConfigRequest();
            grpcClient.readConfig(request, {}, (err, response) => {
                this.editor.setValue(response.getContent());
            });
        }
    }

    created() {
        i18next.on('languageChanged', (lng) => {
            this.$forceUpdate();
        });
    }
}
</script>

<style lang="scss">
@import 'codemirror/lib/codemirror.css';
@import 'codemirror/theme/dracula.css';

.container {
    > div {
        height: 100%;
        .CodeMirror {
            height: 100%;
        }
    }
}
</style>