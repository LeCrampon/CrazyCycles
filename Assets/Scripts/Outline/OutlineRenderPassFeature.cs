using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutlineRenderPassFeature : ScriptableRendererFeature
{
	OutlineRenderPass _renderPass;

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(_renderPass);
	}

	public override void Create()
	{
		_renderPass = new OutlineRenderPass();

		_renderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

}
